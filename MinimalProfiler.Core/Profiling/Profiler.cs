using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using MinimalProfiler.Core.Attributes;
using MinimalProfiler.Core.Logging;
using MinimalProfiler.Core.Profiling.Internal;

namespace MinimalProfiler.Core.Profiling
{
    public class Profiler
    {
        public string Name { get; private set; }

        private bool isRunning;
        private bool isPatched;
        private readonly List<MethodInfo> methods;
        private readonly ILog log;
        private Harmony harmony;
        private Func<ProfilingResult, string> format;

        internal Profiler(string name, ILog log, IEnumerable<Assembly> assemblies, bool runOnBuild, Func<ProfilingResult, string> format)
        {
            this.log = log;
            this.Name = name;
            this.format = format;

            harmony = new Harmony(name);
            methods = new List<MethodInfo>();

            foreach (var assembly in assemblies)
                methods.AddRange(assembly.GetTypes()
                          .SelectMany(t => t.GetMethods())
                          .Where(m =>
                          {
                              var attr = (ProfileMeAttribute)m.GetCustomAttributes(typeof(ProfileMeAttribute), false).FirstOrDefault();
                              return attr != null && (attr.ProfilerName == Name || string.IsNullOrEmpty(attr.ProfilerName));
                          }));
            Log($"Found {methods.Count} profileable methods in {assemblies.Count()} assemblies", LogLevel.Debug);

            if (runOnBuild)
            {
                PatchAll();
                isRunning = true;
            }

            GlobalProfilingState.GetInstance.RegisterProfiler(this);
            Log("Profiler created");
        }

        public void AddProfilingResult(ProfilingResult result)
        {
            var logString = format(result);

            log.Info(logString);
        }

        private void Log(string content, LogLevel level = LogLevel.Verbose, Exception e = null)
        {
            var s = "Profiler: " + content;
            switch (level)
            {
                case LogLevel.Verbose:
                    log.Verbose(s, e);
                    break;
                case LogLevel.Debug:
                    log.Debug(s, e);
                    break;
                case LogLevel.Info:
                    log.Info(s, e);
                    break;
                case LogLevel.Critical:
                    log.Critical(s, e);
                    break;
            }
        }

        public void Start()
        {
            Log("Starting profiling", LogLevel.Debug);

            if (isRunning)
                return;

            if (!isPatched)
                PatchAll();
            isRunning = true;
        }

        public void Stop()
        {
            Log("Stopping profiling", LogLevel.Debug);

            if (!isRunning)
                return;
            isRunning = false;
        }

        private void PatchAll()
        {
            var profilerPrefix = AccessTools.Method(typeof(ProfilingMethods), "StartProfiling");
            var profilerPostfix = AccessTools.Method(typeof(ProfilingMethods), "StopProfiling");

            Log($"Patching with prefix {profilerPrefix.Name} and postfix {profilerPostfix.Name}");
            int patchedMethods = 0;
            foreach (var method in methods)
            {
                try
                {
                    harmony.Patch(method, new HarmonyMethod(profilerPrefix), new HarmonyMethod(profilerPostfix));
                    patchedMethods++;
                    Log($"Patched {method.Name}");
                }
                catch (Exception e)
                {
                    log.Critical($"Failed to patch {method.Name}", e);
                }
            }
            Log($"Patched {patchedMethods} methods", LogLevel.Debug);

            isPatched = true;
        }

        public static ProfilerBuilder Create(string profilerName = "profiler") => new ProfilerBuilder(profilerName);

    }

    public class ProfilerBuilder
    {
        private List<Assembly> Assemblies;
        private ILog log;
        private string name;
        private Func<ProfilingResult, string> format;

        internal ProfilerBuilder(string name)
        {
            Assemblies = new List<Assembly>();
            log = new ConsoleLog();
            this.name = name;
            format = (r => $"{r.DisplayName} took ada {r.Elapsed.Ticks} ticks | {r.Elapsed.Milliseconds} ms to execute");
        }

        public ProfilerBuilder UseAssemblies(params Assembly[] assemblies)
        {
            Assemblies.AddRange(assemblies);

            return this;
        }

        public ProfilerBuilder UseFormat(Func<ProfilingResult, string> format)
        {
            this.format = format;
            return this;
        }

        public ProfilerBuilder UseLog(ILog log)
        {
            this.log = log;
            return this;
        }

        public Profiler Build(bool run = true) => new Profiler(name, log, Assemblies, run, format);
    }
}
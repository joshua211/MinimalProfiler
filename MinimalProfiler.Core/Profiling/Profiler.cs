using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Attributes;
using MinimalProfiler.Core.Profiling.Enums;
using MinimalProfiler.Core.Profiling.Internal;

namespace MinimalProfiler.Core.Profiling
{
    public class Profiler
    {
        public string Name { get; private set; }

        private bool isRunning;
        private bool isPatched;
        private readonly List<PatchMethod> methods;
        private readonly ILogger log;
        private Harmony harmony;
        private Func<ProfilingResult, string> format;

        internal Profiler(string name, ILogger log, IEnumerable<Assembly> assemblies, bool runOnBuild, Func<ProfilingResult, string> format)
        {
            this.log = log;
            this.Name = name;
            this.format = format;

            harmony = new Harmony(name);
            methods = new List<PatchMethod>();

            foreach (var assembly in assemblies)
                methods.AddRange(assembly.GetTypes()
                          .SelectMany(t => t.GetMethods())
                          .Where(m =>
                          {
                              var attr = (IProfilerAttribute)m.GetCustomAttributes().Where(a => a is IProfilerAttribute).FirstOrDefault();
                              return attr != null && (attr.ProfilerName == Name || string.IsNullOrEmpty(attr.ProfilerName));
                          }).Select(m =>
                          {
                              //TODO make this better, don't return attributes twice
                              var attr = m.GetCustomAttributes().Where(a => a is IProfilerAttribute).FirstOrDefault();
                              if (attr is ProfileMeAttribute)
                                  return new PatchMethod(m, ProfilingPatchType.Normal);
                              return new PatchMethod(m, ProfilingPatchType.Async);
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

            log.LogInformation(logString);
        }

        private void Log(string content, LogLevel level = LogLevel.Trace, Exception e = null)
        {
            var s = "Profiler: " + content;
            log.Log(level, content, e);
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
            var profilerPostfixAsync = AccessTools.Method(typeof(ProfilingMethods), "StopProfilingAsync");

            Log($"Patching with prefix {profilerPrefix.Name} and postfix {profilerPostfix.Name}");
            int patchedMethods = 0;
            foreach (var info in methods)
            {
                var method = info.Method;
                try
                {
                    var postfix = info.PatchType == ProfilingPatchType.Normal ? profilerPostfix : profilerPostfixAsync;
                    harmony.Patch(info.Method, new HarmonyMethod(profilerPrefix), new HarmonyMethod(postfix));
                    patchedMethods++;
                    Log($"Patched {method.Name}");
                }
                catch (Exception e)
                {
                    Log($"Failed to patch {method.Name}", LogLevel.Critical, e);
                }
            }
            Log($"Patched {patchedMethods} methods", LogLevel.Debug);

            isPatched = true;
        }

        public static ProfilerBuilder Create(string profilerName = "profiler") => new ProfilerBuilder(profilerName);

    }
}
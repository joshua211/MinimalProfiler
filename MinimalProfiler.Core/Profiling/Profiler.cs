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
    /// <summary>
    /// Main class of this application which is used to patch methods, evaluate results and print logs
    /// </summary>
    public class Profiler
    {
        public string Name { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPatched { get; private set; }
        public ILogger log { get; private set; }
        public Func<ProfilingResult, string> format { get; private set; }

        private List<PatchMethod> methods;
        public IEnumerable<string> PatchedMethods { get => methods?.Select(m => m.Method.Name); }
        private Harmony harmony;

        /// <summary>
        /// The constructor used for dependency injection
        /// </summary>
        /// <param name="logger"></param>
        public Profiler(ILogger<Profiler> logger)
                : this("Profiler", logger, new[] { Assembly.GetEntryAssembly() }, false
                , r => $"{r.DisplayName} took {r.Time.Ticks} ticks | {r.Time.Milliseconds} ms to execute")
        {

        }

        internal Profiler(string name, ILogger log, IEnumerable<Assembly> assemblies, bool runOnBuild, Func<ProfilingResult, string> format)
        {
            this.log = log;
            this.Name = name;
            this.format = format;

            harmony = new Harmony(name);
            methods = new List<PatchMethod>();

            if (GlobalProfilingState.GetInstance.DoesProfilerExists(name))
                throw new Exception($"A profiler with the name '{name}' already exists for this application");

            if (!assemblies.Any())
                Log("No assemblies specified, so no profiling can be done", LogLevel.Warning);

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
                IsRunning = true;
            }

            GlobalProfilingState.GetInstance.RegisterProfiler(this);
            Log($"Profiler created: {log}, {assemblies}");
        }

        internal void AddProfilingResult(ProfilingResult result)
        {
            var logString = format(result);

            Log(logString, LogLevel.Information);
        }

        private void Log(string content, LogLevel level = LogLevel.Trace, Exception e = null)
        {
            log.Log(level, content, e);
        }

        /// <summary>
        /// Starts the profiling and patches all methods if not already done
        /// </summary>
        public void Start()
        {
            Log("Starting profiling", LogLevel.Debug);

            if (IsRunning)
                return;

            if (!IsPatched)
                PatchAll();
            IsRunning = true;
        }

        /// <summary>
        /// Stops profiling
        /// </summary>
        public void Stop()
        {
            Log("Stopping profiling", LogLevel.Debug);

            if (!IsRunning)
                return;
            IsRunning = false;
        }

        /// <summary>
        /// Removes the profiler from the global state and unpatches all methods
        /// </summary>
        public void Remove()
        {
            Stop();
            GlobalProfilingState.GetInstance.RemoveProfiler(Name);
            //TODO unpatch all methods
            IsPatched = false;
        }

        private void PatchAll()
        {
            var profilerPrefix = AccessTools.Method(typeof(ProfilingMethods), "StartProfiling");
            var profilerPostfix = AccessTools.Method(typeof(ProfilingMethods), "StopProfiling");
            var profilerPostfixAsync = AccessTools.Method(typeof(ProfilingMethods), "StopProfilingAsync");

            int patchedMethods = 0;
            foreach (var info in methods)
            {
                var method = info.Method;
                try
                {
                    var postfix = info.PatchType == ProfilingPatchType.Normal ? profilerPostfix : profilerPostfixAsync;
                    Log($"Trying to patch {method.Name} with {profilerPrefix.Name} and {postfix.Name}");
                    harmony.Patch(info.Method, new HarmonyMethod(profilerPrefix), new HarmonyMethod(postfix));
                    patchedMethods++;
                }
                catch (Exception e)
                {
                    Log($"Failed to patch {method.Name}", LogLevel.Critical, e);
                }
            }
            Log($"Patched {patchedMethods} methods", LogLevel.Debug);

            IsPatched = true;
        }

        /// <summary>
        /// Create a new instance of ProfilerBuilder to create a Profiler with fluid syntax
        /// </summary>
        /// <param name="profilerName">The unique name of the profiler</param>
        /// <returns>A default ProfilerBuilder instance with initial settings</returns>
        public static ProfilerBuilder Create(string profilerName = "profiler") => new ProfilerBuilder(profilerName);

    }
}
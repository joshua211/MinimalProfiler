using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Handlers;
using MinimalProfiler.Core.Internal;
using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core
{
    /// <summary>
    /// Class to build the Profiler with fluid syntax
    /// </summary>
    public class ProfilerBuilder
    {
        private List<Assembly> Assemblies;
        private ILogger log;
        private string name;
        private Func<ProfilingResult, LogMessage> format;
        private List<IProfilingResultHandler> handlers;

        internal ProfilerBuilder(string name)
        {
            Assemblies = new List<Assembly>();
            var provider = LoggerFactory.Create(b => b.AddConsole());
            log = provider.CreateLogger("Profiling");
            this.name = name;
            format = r =>
                new LogMessage($"{r.DisplayName} took {r.Time.Ticks} ticks | {r.Time.Milliseconds} ms to execute",
                    LogLevel.Information);
            handlers = new List<IProfilingResultHandler>();
        }

        /// <summary>
        /// Tells the profiler which assemblies to search for methods that have a [ProfileMe] attribute
        /// </summary>
        /// <param name="assemblies">A list of assemblies</param>
        /// <returns></returns>
        public ProfilerBuilder UseAssemblies(params Assembly[] assemblies)
        {
            Assemblies.AddRange(assemblies);

            return this;
        }

        /// <summary>
        /// Add a new result handler to the profiler. LogResultHandler is added by default.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ProfilerBuilder UseResultHandler(IProfilingResultHandler handler)
        {
            handlers.Add(handler);
            
            return this;
        }

        /// <summary>
        /// Tells the profiler which format to use for logging. 
        /// Default is 'DisplayName took {ticks} ticks | {ms} ms to execute}'
        /// </summary>
        /// <param name="format">The function used to create the log string</param>
        /// <returns></returns>
        public ProfilerBuilder UseFormat(Func<ProfilingResult, LogMessage> newFormat)
        {
            format = newFormat;
            return this;
        }

        /// <summary>
        /// Tells the profiler which ILogger to use. Default logger is the console
        /// </summary>
        /// <param name="logger">Any ILogger implementation</param>
        /// <returns></returns>
        public ProfilerBuilder UseLogger(ILogger logger)
        {
            this.log = logger;
            return this;
        }

        /// <summary>
        /// Build the profiler and optionally tells him to patch all methods and start profiling
        /// </summary>
        /// <param name="run">Controlls wether the profiler should patch and start profiling on build</param>
        /// <returns>A new customized Profiler instance</returns>
        public IProfiler Build(bool run = true)
        {
            handlers.Add(new LogResultHandler(format, log));
            return new Profiler(name, log, Assemblies, run, handlers);
        }

        /// <summary>
        /// Create a new instance of ProfilerBuilder to create a Profiler with fluid syntax
        /// </summary>
        /// <param name="profilerName">The unique name of the profiler</param>
        /// <returns>A default ProfilerBuilder instance with initial settings</returns>
        public static ProfilerBuilder Create(string profilerName = "profiler") => new ProfilerBuilder(profilerName);
    }
}
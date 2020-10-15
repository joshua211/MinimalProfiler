using System;
using System.Collections.Generic;
using System.Reflection;
using MinimalProfiler.Core.Logging;

namespace MinimalProfiler.Core.Profiling
{
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
            format = (r => $"{r.DisplayName} took {r.Time.Ticks} ticks | {r.Time.Milliseconds} ms to execute");
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
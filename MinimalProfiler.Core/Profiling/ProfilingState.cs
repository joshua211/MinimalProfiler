using System;
using System.Diagnostics;

namespace MinimalProfiler.Core.Profiling
{
    public class ProfilingState
    {
        public Stopwatch Watch { get; set; }
        public string DisplayName { get; set; }
        public string ProfilerName { get; set; }

        public ProfilingState(string displayName, string profilerName)
        {
            DisplayName = displayName;
            Watch = new Stopwatch();
            ProfilerName = profilerName;
        }

        public void Start()
        {
            Watch.Start();
        }

        public ProfilingResult Stop()
        {
            Watch.Stop();
            return new ProfilingResult(DisplayName, new ProfilingTime(Watch));
        }
    }
}
using System;

namespace MinimalProfiler.Core.Profiling
{
    public class ProfilingResult
    {
        public TimeSpan Elapsed { get; private set; }
        public string DisplayName { get; private set; }

        public ProfilingResult(TimeSpan elapsed, string displayName)
        {
            Elapsed = elapsed;
            DisplayName = displayName;
        }
    }
}
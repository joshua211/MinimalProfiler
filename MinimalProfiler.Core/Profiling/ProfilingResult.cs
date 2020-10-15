using System;

namespace MinimalProfiler.Core.Profiling
{
    public class ProfilingResult
    {
        /* We use ProfilingTime because for some reason,
         Watch.Elapsed and Watch.ElapsedTicks is not the same in an async context */

        public ProfilingTime Time { get; private set; }
        public string DisplayName { get; private set; }

        public ProfilingResult(string displayName, ProfilingTime time)
        {
            DisplayName = displayName;
            Time = time;
        }
    }
}
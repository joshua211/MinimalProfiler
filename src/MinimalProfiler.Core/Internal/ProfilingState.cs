using System.Diagnostics;

namespace MinimalProfiler.Core.Internal
{
    internal class ProfilingState
    {
        private readonly Stopwatch Watch;
        public string DisplayName { get; set; }
        public string ProfilerName { get; set; }

        public ProfilingState(string displayName, string profilerName)
        {
            DisplayName = displayName;
            ProfilerName = profilerName;
            Watch = new Stopwatch();
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
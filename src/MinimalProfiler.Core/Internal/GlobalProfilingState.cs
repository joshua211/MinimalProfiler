using System.Collections.Generic;
using System.Linq;

namespace MinimalProfiler.Core.Internal
{
    internal class GlobalProfilingState
    {
        private static GlobalProfilingState instance;
        public static GlobalProfilingState Instance => instance ??= new GlobalProfilingState();

        private readonly Dictionary<string, Profiler> allProfilers;

        private GlobalProfilingState()
        {
            allProfilers = new Dictionary<string, Profiler>();
        }

        public void RegisterProfiler(Profiler profiler) => allProfilers[profiler.Name] = profiler;

        public bool DoesProfilerExists(string name) => allProfilers.Keys.Contains(name);

        public Profiler GetProfiler(string name)
        {
            return allProfilers.Count > 1 ? allProfilers[name] : GetDefault();
        }

        public void RemoveProfiler(string name) => allProfilers.Remove(name);

        public Profiler GetDefault() => allProfilers.Values.First();


    }
}
using System.Collections.Generic;
using System.Linq;

namespace MinimalProfiler.Core.Internal
{
    public class GlobalProfilingState
    {
        private static GlobalProfilingState instance;
        public static GlobalProfilingState Instance => instance ??= new GlobalProfilingState();

        private readonly Dictionary<string, IProfiler> allProfilers;

        private GlobalProfilingState()
        {
            allProfilers = new Dictionary<string, IProfiler>();
        }

        public void RegisterProfiler(IProfiler profiler) => allProfilers[profiler.Name] = profiler;

        public bool DoesProfilerExists(string name) => allProfilers.Keys.Contains(name);

        public IProfiler GetProfiler(string name)
        {
            return allProfilers.Count > 1 ? allProfilers[name] : GetDefault();
        }

        public void RemoveProfiler(string name) => allProfilers.Remove(name);

        public IProfiler GetDefault() => allProfilers.Values.First();


    }
}
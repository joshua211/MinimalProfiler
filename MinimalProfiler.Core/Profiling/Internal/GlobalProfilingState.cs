using System.Collections.Generic;
using System.Linq;

namespace MinimalProfiler.Core.Profiling.Internal
{
    internal class GlobalProfilingState
    {
        private static GlobalProfilingState _instance;
        public static GlobalProfilingState GetInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalProfilingState();
                return _instance;
            }
        }

        private Dictionary<string, Profiler> AllProfilers;

        public GlobalProfilingState()
        {
            AllProfilers = new Dictionary<string, Profiler>();
        }

        public void RegisterProfiler(Profiler profiler) => AllProfilers[profiler.Name] = profiler;

        public bool DoesProfilerExists(string name) => AllProfilers.Keys.Contains(name);

        public Profiler GetProfiler(string name)
        {
            if (AllProfilers.Count > 1)
                return AllProfilers[name];
            return GetDefault();
        }

        public Profiler GetDefault() => AllProfilers.Values.First();


    }
}
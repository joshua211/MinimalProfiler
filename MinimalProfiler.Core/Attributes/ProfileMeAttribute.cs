using System.Runtime.CompilerServices;

namespace MinimalProfiler.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ProfileMeAttribute : System.Attribute
    {
        public string DisplayName { get; private set; }
        public string ProfilerName { get; private set; }


        public ProfileMeAttribute([CallerMemberName] string displayName = "", string profilerName = null)
        {
            DisplayName = displayName;
            ProfilerName = profilerName;
        }
    }
}
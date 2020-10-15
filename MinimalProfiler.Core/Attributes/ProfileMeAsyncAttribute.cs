using System.Runtime.CompilerServices;

namespace MinimalProfiler.Core.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ProfileMeAsyncAttribute : System.Attribute, IProfilerAttribute
    {
        public string DisplayName { get; private set; }
        public string ProfilerName { get; private set; }


        public ProfileMeAsyncAttribute([CallerMemberName] string displayName = "", string profilerName = null)
        {
            DisplayName = displayName;
            ProfilerName = profilerName;
        }
    }
}
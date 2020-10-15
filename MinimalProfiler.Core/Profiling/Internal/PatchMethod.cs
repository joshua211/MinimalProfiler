using System.Reflection;
using MinimalProfiler.Core.Profiling.Enums;

namespace MinimalProfiler.Core.Profiling.Internal
{
    public class PatchMethod
    {
        public MethodBase Method { get; private set; }
        public ProfilingPatchType PatchType { get; private set; }

        public PatchMethod(MethodBase method, ProfilingPatchType patchType)
        {
            Method = method;
            PatchType = patchType;
        }
    }


}
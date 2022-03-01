using System.Reflection;
using MinimalProfiler.Core.Enums;

namespace MinimalProfiler.Core.Internal.Patch
{
    internal class PatchMethod
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
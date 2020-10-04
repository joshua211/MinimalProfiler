using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using MinimalProfiler.Core.Attributes;

namespace MinimalProfiler.Core.Profiling.Internal
{
    internal static class ProfilingMethods
    {
        public static void StartProfiling(out ProfilingState __state, MethodInfo __originalMethod)
        {
            var attr = (ProfileMeAttribute)__originalMethod.GetCustomAttribute(typeof(ProfileMeAttribute));
            __state = new ProfilingState(attr.DisplayName, attr.ProfilerName);
            __state.Start();
        }

        public static void StopProfiling(ProfilingState __state)
        {
            var globalState = GlobalProfilingState.GetInstance;
            var result = __state.Stop();
            var profiler = string.IsNullOrEmpty(__state.ProfilerName) ?
                                            globalState.GetDefault() :
                                            globalState.GetProfiler(__state.ProfilerName);
            profiler.AddProfilingResult(result);
        }

    }
}
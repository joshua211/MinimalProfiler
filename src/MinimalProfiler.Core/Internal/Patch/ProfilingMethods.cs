using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MinimalProfiler.Core.Attributes;

namespace MinimalProfiler.Core.Internal.Patch
{
    internal static class ProfilingMethods
    {
        public static void StartProfiling(out ProfilingState __state, MethodInfo __originalMethod)
        {
            var attr = (IProfilerAttribute)__originalMethod.GetCustomAttributes().Where(a => a is IProfilerAttribute).FirstOrDefault();
            __state = new ProfilingState(attr.DisplayName, attr.ProfilerName);
            __state.Start();
        }

        public static void StopProfiling(ProfilingState __state)
        {
            var globalState = GlobalProfilingState.Instance;
            var result = __state.Stop();
            var profiler = string.IsNullOrEmpty(__state.ProfilerName) ?
                                            globalState.GetDefault() :
                                            globalState.GetProfiler(__state.ProfilerName);
            profiler.AddProfilingResult(result);
        }

        public static void StopProfilingAsync(ref Task __result, ProfilingState __state)
        {
            var contin =
                __result.ContinueWith(T =>
                {
                    var globalState = GlobalProfilingState.Instance;
                    var result = __state.Stop();
                    var profiler = string.IsNullOrEmpty(__state.ProfilerName) ?
                                                    globalState.GetDefault() :
                                                    globalState.GetProfiler(__state.ProfilerName);
                    profiler.AddProfilingResult(result);
                });

            contin.Wait();
        }

    }
}
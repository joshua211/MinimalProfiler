using System.Threading;
using System.Threading.Tasks;
using MinimalProfiler.Core.Attributes;

namespace MinimalProfiler.Test
{
    public class TestBase
    {
        [ProfileMe]
        public void DoSomething(int timeout)
        {
            Thread.Sleep(timeout);
        }

        [ProfileMe("SomethingDifferent")]
        public void DoSomethingDifferent(int timeout)
        {
            Thread.Sleep(timeout);
        }

        [ProfileMeAsync]
        public async Task DoSomethingAsync(int timeout)
        {
            await Task.Delay(timeout);
        }

        [ProfileMe(profilerName: "whoknows")]
        public void DoSomethingWithWrongProfiler(int timeout)
        {
            Thread.Sleep(timeout);
        }

        [ProfileMe(profilerName: "profiler_1")]
        public void DoSomethingForProfiler_1(int timeout)
        {
            Thread.Sleep(timeout);
        }

        [ProfileMe(profilerName: "profiler_2")]
        public void DoSomethingForProfiler_2(int timeout)
        {
            Thread.Sleep(timeout);
        }
    }
}
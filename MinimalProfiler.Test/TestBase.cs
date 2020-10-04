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

        [ProfileMe]
        public async Task DoSomethingAsync(int timeout)
        {
            await Task.Delay(timeout);
        }

        [ProfileMe(profilerName: "whoknows")]
        public void DoSomethingWithWrongProfiler(int timeout)
        {
            Thread.Sleep(timeout);
        }
    }
}
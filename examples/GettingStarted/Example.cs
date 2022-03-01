using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using MinimalProfiler.Core.Attributes;

namespace GettingStarted
{
    public class Example
    {
        [ProfileMe]
        public void DoSomething()
        {
            for (long i = 0; i < int.MaxValue; i++)
            {
                //something
            }
        }

        [ProfileMeAsync]
        public async Task AwaitSomething()
        {
            await Task.Delay(2000);
        }
    }
}
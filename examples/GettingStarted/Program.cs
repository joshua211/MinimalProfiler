using System.Threading.Tasks;
using MinimalProfiler.Core;

namespace GettingStarted
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var profiler = ProfilerBuilder.Create()
                                    .UseAssemblies(typeof(Example).Assembly)
                                    .Build();

            var example = new Example();


            example.DoSomething();
            await example.AwaitSomething();
        }
    }
}

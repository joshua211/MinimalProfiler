using System;
using MinimalProfiler.Core.Profiling;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var profiler = Profiler.Create()
                                    .UseAssemblies(typeof(Example).Assembly)
                                    .Build();

            var example = new Example();


            example.DoSomething();
        }
    }
}

using System;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Profiling;

namespace MinimalProfiler.Test.Fixtures
{
    public class DualFixture : IDisposable
    {
        public DebugLog Log_1 { get; set; }
        public DebugLog Log_2 { get; set; }

        public DualFixture()
        {
            Log_1 = new DebugLog(LogLevel.Information);
            Log_2 = new DebugLog(LogLevel.Information);

            Profiler.Create("profiler_1").UseAssemblies(typeof(TestBase).Assembly).UseLog(Log_1).Build();
            Profiler.Create("profiler_2").UseAssemblies(typeof(TestBase).Assembly).UseLog(Log_2).Build();
        }


        public void Dispose()
        {
        }
    }
}
using System;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Profiling;

namespace MinimalProfiler.Test.Fixtures
{
    public class SingleFixture : IDisposable
    {
        public DebugLog Log { get; set; }

        public SingleFixture()
        {
            Log = new DebugLog(LogLevel.Trace);

            Profiler.Create().UseAssemblies(typeof(TestBase).Assembly).UseLog(Log).Build();
        }

        public void Dispose()
        {

        }
    }
}
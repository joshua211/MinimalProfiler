using System;
using MinimalProfiler.Core.Profiling;
using Xunit;

namespace MinimalProfiler.Test
{
    public class FormatTest
    {
        private Profiler profiler;
        private readonly DebugLog log;
        private readonly TestBase testBase;

        public FormatTest()
        {
            log = new DebugLog();
            profiler = Profiler.Create()
                                .UseFormat(r => $"test")
                                .UseLog(log)
                                .UseAssemblies(typeof(TestBase).Assembly)
                                .Build();
            testBase = new TestBase();
        }

        [Fact]
        public void LogShouldPrintHello()
        {
            var excpected = "test";
            testBase.DoSomething(100);

            Assert.Equal(excpected, log.LogStack.Pop().Entry);
        }
    }
}
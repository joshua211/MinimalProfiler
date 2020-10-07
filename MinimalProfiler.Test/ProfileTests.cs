using System;
using System.Threading.Tasks;
using MinimalProfiler.Core.Logging;
using MinimalProfiler.Core.Profiling;
using Xunit;

namespace MinimalProfiler.Test
{
    public class ProfileTests
    {
        private readonly TestBase testBase;
        private readonly Profiler profiler;
        private readonly DebugLog log;

        public ProfileTests()
        {
            log = new DebugLog();
            profiler = Profiler.Create()
                                .UseLog(log)
                                .UseAssemblies(typeof(TestBase).Assembly)
                                .Build();

            testBase = new TestBase();
        }

        [Fact]
        public void DoSomethingShouldProduceValidLogEntry()
        {
            var expectedLogLevel = LogLevel.Info;
            var excpectedMethodName = "DoSomething";
            var timeout = 1000;

            testBase.DoSomething(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.Entry);
        }

        [Fact]
        public async Task DoSomethingAsyncTest()
        {
            var expectedLogLevel = LogLevel.Info;
            var excpectedMethodName = "DoSomething";
            var timeout = 2000;

            await testBase.DoSomethingAsync(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.Entry);
        }

        [Fact]
        public void DoSomethingDifferentShouldUseDifferentName()
        {
            var expectedLogLevel = LogLevel.Info;
            var excpectedMethodName = "SomethingDifferent";
            var timeout = 1000;

            testBase.DoSomethingDifferent(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.Entry);
        }
    }
}

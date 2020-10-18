using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Profiling;
using MinimalProfiler.Test.Fixtures;
using Xunit;

namespace MinimalProfiler.Test
{
    public class ProfileTests : IClassFixture<SingleFixture>
    {
        private readonly TestBase testBase;
        private readonly Profiler profiler;
        private readonly DebugLog log;

        public ProfileTests(SingleFixture fixture)
        {
            log = fixture.Log;

            testBase = new TestBase();
        }

        [Fact]
        public void DoSomethingShouldProduceValidLogEntry()
        {
            var expectedLogLevel = LogLevel.Information;
            var excpectedMethodName = "DoSomething";
            var timeout = 100;

            testBase.DoSomething(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.LogMessage);
        }

        [Fact]
        public async Task DoSomethingAsyncTest()
        {
            var expectedLogLevel = LogLevel.Information;
            var excpectedMethodName = "DoSomething";
            var timeout = 200;

            await testBase.DoSomethingAsync(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.LogMessage);
        }

        [Fact]
        public void DoSomethingDifferentShouldUseDifferentName()
        {
            var expectedLogLevel = LogLevel.Information;
            var excpectedMethodName = "SomethingDifferent";
            var timeout = 100;

            testBase.DoSomethingDifferent(timeout);

            var logResult = log.LogStack.Pop();
            Assert.Equal(expectedLogLevel, logResult.Level);
            Assert.Contains(excpectedMethodName, logResult.LogMessage);
        }
    }
}

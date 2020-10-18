using MinimalProfiler.Core.Profiling;
using MinimalProfiler.Test.Fixtures;
using Xunit;

namespace MinimalProfiler.Test
{
    public class DualProfilingTest : IClassFixture<DualFixture>
    {
        private DebugLog log1;
        private DebugLog log2;
        private TestBase testBase;

        public DualProfilingTest(DualFixture fixture)
        {
            log1 = fixture.Log_1;
            log2 = fixture.Log_2;
            testBase = new TestBase();
        }

        [Fact]
        public void Profiler1MethodShouldProduceOneLog()
        {
            var expectedLogs = 1;

            testBase.DoSomethingForProfiler_1(100);

            Assert.Equal(expectedLogs, log1.Count);
            Assert.Equal(0, log2.Count);

            ClearLogs();
        }

        [Fact]
        public void Profiler2MethodShouldProduceOneLog()
        {
            var expectedLogs = 1;

            testBase.DoSomethingForProfiler_2(100);

            Assert.Equal(expectedLogs, log2.Count);
            Assert.Equal(0, log1.Count);

            ClearLogs();
        }

        [Fact]
        public void BothMethodsShouldOnlyRaiseOneProfiler()
        {
            var expectedLogs = 1;

            testBase.DoSomethingForProfiler_1(100);
            testBase.DoSomethingForProfiler_2(100);

            Assert.Equal(expectedLogs, log1.Count);
            Assert.Equal(expectedLogs, log2.Count);

            ClearLogs();
        }


        private void ClearLogs()
        {
            log1.LogStack.Clear();
            log2.LogStack.Clear();
        }


    }
}
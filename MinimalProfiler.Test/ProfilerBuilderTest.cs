using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Profiling;
using NSubstitute;
using Xunit;

namespace MinimalProfiler.Test
{
    public class ProfilerBuilderTest
    {
        [Fact]
        public void ProfilerBuilderShouldBuildDefault()
        {
            var profiler = Profiler.Create().Build(false);

            profiler.Name.Should().Be("profiler");
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderWithUniqueName()
        {
            var name = "test";
            var profiler = Profiler.Create(name).Build(false);

            profiler.Name.Should().Be(name);
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderWithCustomLog()
        {
            var log = Substitute.For<ILogger>();
            var profiler = Profiler.Create().UseLog(log).Build(false);

            profiler.log.Should().Be(log);
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderWithAssemblyList()
        {
            var testMethods = new string[] { "DoSomething", "DoSomethingAsync" };
            var profiler = Profiler.Create().UseAssemblies(typeof(TestMethods).Assembly).Build(false);

            profiler.PatchedMethods.Should().BeEquivalentTo(testMethods);
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderWithCustomFormat()
        {
            var format = new Func<ProfilingResult, string>(s => "test");
            var profiler = Profiler.Create().UseFormat(format).Build();

            profiler.format.Should().Be(format);
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderShouldNotRunAtCreation()
        {
            var shouldRun = false;
            var profiler = Profiler.Create().Build(shouldRun);

            profiler.IsPatched.Should().Be(shouldRun);
            profiler.IsRunning.Should().Be(shouldRun);
            profiler.Remove();
        }

        [Fact]
        public void ProfilerBuilderShouldRunAtCreation()
        {
            var shouldRun = true;
            var profiler = Profiler.Create().Build(shouldRun);

            profiler.IsPatched.Should().Be(shouldRun);
            profiler.IsRunning.Should().Be(shouldRun);
            profiler.Remove();
        }
    }
}
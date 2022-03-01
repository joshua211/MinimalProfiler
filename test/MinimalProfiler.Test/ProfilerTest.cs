using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Profiling;
using NSubstitute;
using Xunit;

namespace MinimalProfiler.Test
{
    public class ProfilerTest
    {

        public ProfilerTest()
        {

        }

        [Fact]
        public void ProfilerStart()
        {
            var profiler = GetProfiler("test", false);
            profiler.Start();

            profiler.IsRunning.Should().Be(true);
            profiler.IsPatched.Should().Be(true);

            profiler.Remove();
        }

        [Fact]
        public void ProfilerStop()
        {
            var profiler = GetProfiler("test", true);
            profiler.Stop();

            profiler.IsRunning.Should().Be(false);
            profiler.IsPatched.Should().Be(true);
        }

        [Fact]
        public void RemoveProfiler()
        {
            var profiler = GetProfiler("test", true);
            profiler.Remove();

            profiler.IsRunning.Should().Be(false);
            profiler.IsPatched.Should().Be(false);
        }

        [Fact]
        public void ProfilerShouldFindBothMethods()
        {
            var profiler = GetProfiler("test", true);

            profiler.PatchedMethods.Should().Contain("DoSomething");
            profiler.PatchedMethods.Should().Contain("DoSomethingAsync");
        }

        private Profiler GetProfiler(string name, bool run) => Profiler.Create(name)
                .UseAssemblies(typeof(ProfilerTest).Assembly)
                .Build(run);


    }
}
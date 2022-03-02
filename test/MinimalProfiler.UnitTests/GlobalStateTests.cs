using FluentAssertions;
using MinimalProfiler.Core;
using MinimalProfiler.Core.Internal;
using NSubstitute;
using Xunit;

namespace MinimalProfiler.UnitTests;

public class GlobalStateTests
{
    [Fact]
    public void RegisterProfilerAddsNewProfiler()
    {
        var profilerName = "profiler";
        var profiler = Substitute.For<IProfiler>();
        profiler.Name.Returns(profilerName);
        
        GlobalProfilingState.Instance.RegisterProfiler(profiler);

        GlobalProfilingState.Instance.GetProfiler(profilerName).Should().Be(profiler);
    }
}
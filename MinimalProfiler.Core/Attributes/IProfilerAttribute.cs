namespace MinimalProfiler.Core.Attributes
{
    public interface IProfilerAttribute
    {
        string DisplayName { get; }
        string ProfilerName { get; }
    }
}
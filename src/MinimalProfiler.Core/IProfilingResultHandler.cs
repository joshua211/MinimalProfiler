using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core;

public interface IProfilingResultHandler
{
    void Handle(ProfilingResult result);
}
using System;
using MinimalProfiler.Core.Internal;
using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core;

public interface IProfiler
{
    void Start();
    void Stop();
    void Remove();
    void AddProfilingResult(ProfilingResult result);
    string Name { get; }
    bool IsPatched { get; }
    bool IsRunning { get; }
    Func<ProfilingResult, string> Format { get; }
}
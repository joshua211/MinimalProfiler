using System;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core.Handlers;

public class LogResultHandler : IProfilingResultHandler
{
    private readonly Func<ProfilingResult, LogMessage> format;
    private readonly ILogger logger;

    public LogResultHandler(Func<ProfilingResult, LogMessage> format, ILogger logger)
    {
        this.format = format;
        this.logger = logger;
    }

    public void Handle(ProfilingResult result)
    {
        var message = format(result);
        logger.Log(message.Level, message.Message);
    }
}
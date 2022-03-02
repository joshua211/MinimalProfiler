using Microsoft.Extensions.Logging;

namespace MinimalProfiler.Core.Models;

public class LogMessage
{
    public string Message { get; private set; }
    public LogLevel Level { get; private set; }

    public LogMessage(string message, LogLevel level)
    {
        Message = message;
        Level = level;
    }
}
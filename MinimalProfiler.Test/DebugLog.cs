using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MinimalProfiler.Test
{
    public class DebugLog : ILogger<DebugLogEntry>, IDisposable
    {
        public Stack<DebugLogEntry> LogStack { get; private set; }
        public bool HasError { get; private set; }
        public bool HasWarning { get; private set; }
        public bool HasCritical { get; private set; }

        public DebugLog()
        {
            LogStack = new Stack<DebugLogEntry>();
        }

        public IDisposable BeginScope<TState>(TState state) => this;

        public void Dispose()
        {

        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logString = formatter(state, exception);
            LogStack.Push(new DebugLogEntry(logLevel, logString));

            if (logLevel == LogLevel.Critical)
                HasCritical = true;
            else if (logLevel == LogLevel.Error)
                HasError = true;
            else if (logLevel == LogLevel.Warning)
                HasWarning = true;
        }
    }

    public class DebugLogEntry
    {
        public LogLevel Level { get; private set; }
        public string LogMessage { get; private set; }

        public DebugLogEntry(LogLevel level, string logMessage)
        {
            Level = level;
            LogMessage = logMessage;
        }
    }
}
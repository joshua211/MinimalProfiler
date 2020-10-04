using System;
using System.Collections.Generic;
using MinimalProfiler.Core.Logging;

namespace MinimalProfiler.Test
{
    public class DebugLog : ILog
    {
        public Stack<DebugLogEntry> LogStack { get; private set; }

        public DebugLog()
        {
            LogStack = new Stack<DebugLogEntry>();
        }

        public void Critical(string s, Exception e = null)
        {
            LogStack.Push(new DebugLogEntry(s, LogLevel.Critical));
        }

        public void Debug(string s, Exception e = null)
        {
            LogStack.Push(new DebugLogEntry(s, LogLevel.Debug));
        }

        public void Info(string s, Exception e = null)
        {
            LogStack.Push(new DebugLogEntry(s, LogLevel.Info));
        }

        public void Verbose(string s, Exception e = null)
        {
            LogStack.Push(new DebugLogEntry(s, LogLevel.Verbose));
        }
    }

    public class DebugLogEntry
    {
        public string Entry { get; private set; }
        public LogLevel Level { get; private set; }

        public DebugLogEntry(string entry, LogLevel level)
        {
            Entry = entry;
            Level = level;
        }
    }
}
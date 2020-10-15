using System;

namespace MinimalProfiler.Core.Logging
{
    public interface ILog
    {
        //TODO replace this with microsoft logging
        void Debug(string s, Exception e = null);
        void Info(string s, Exception e = null);
        void Critical(string s, Exception e = null);
        void Verbose(string s, Exception e = null);
    }
}
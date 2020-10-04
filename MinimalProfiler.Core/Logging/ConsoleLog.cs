using System;

namespace MinimalProfiler.Core.Logging
{
    public class ConsoleLog : ILog
    {
        public void Critical(string s, Exception e = null)
        {
            System.Console.WriteLine("[CRIT] " + s, e);
        }

        public void Debug(string s, Exception e = null)
        {
            System.Console.WriteLine("[DEB] " + s, e);
        }

        public void Info(string s, Exception e = null)
        {
            System.Console.WriteLine("[INF] " + s, e);
        }

        public void Verbose(string s, Exception e = null)
        {
            System.Console.WriteLine("[VER] " + s, e);
        }
    }
}
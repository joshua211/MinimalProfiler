using System.Diagnostics;
namespace MinimalProfiler.Core.Profiling
{
    public class ProfilingTime
    {
        public long Ticks { get; private set; }
        public long Milliseconds { get; private set; }
        public int Seconds { get; private set; }

        public ProfilingTime(long ticks, long milliseconds, int seconds)
        {
            Ticks = ticks;
            Milliseconds = milliseconds;
            Seconds = seconds;
        }

        public ProfilingTime(Stopwatch watch)
        {
            Ticks = watch.ElapsedTicks;
            Milliseconds = watch.ElapsedMilliseconds;
            Seconds = (int)(Milliseconds / 1000);
        }
    }
}
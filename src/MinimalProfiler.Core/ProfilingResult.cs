using MinimalProfiler.Core.Internal;

namespace MinimalProfiler.Core
{
    /// <summary>
    /// The result of a profiling execution
    /// </summary>
    public class ProfilingResult
    {
        /// <summary>
        /// The time the method took to execute.
        /// We use this because Watch.Elapsed and Watch.ElapsedTicks is not the same in an async context
        /// </summary>
        /// <value></value>
        public ProfilingTime Time { get; private set; }
        /// <summary>
        /// The name which will be displayed in the logs
        /// </summary>
        /// <value></value>
        public string DisplayName { get; private set; }

        public ProfilingResult(string displayName, ProfilingTime time)
        {
            DisplayName = displayName;
            Time = time;
        }
    }
}
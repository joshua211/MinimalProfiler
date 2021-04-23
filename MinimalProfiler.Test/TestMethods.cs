using System.Threading;
using MinimalProfiler.Core.Attributes;

namespace MinimalProfiler.Test
{
    public class TestMethods
    {
        [ProfileMe]
        public void DoSomething()
        {
            System.Console.WriteLine("Something");
        }

        [ProfileMeAsync]
        public void DoSomethingAsync()
        {
            System.Console.WriteLine("SomethingAsync");
        }
    }
}
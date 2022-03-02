using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Attributes;
using MinimalProfiler.Core.Extensions;

namespace DependencyInjection;

public class Program
{
    private static Random random = new Random();
    
    public static void Main()
    {
        var thread = new Thread(DoSomething);
        var host = Host.CreateDefaultBuilder().ConfigureServices(collection =>
        {
            collection.AddLogging(builder => builder.AddConsole());
            collection.AddDefaultProfiler();
        }).Build();
        
        thread.Start();
        host.Start();
    }

    private static void DoSomething()
    {
        for (var j = 0; j < 10; j++)
        {
            Sleep();
        }
    }

    [ProfileMe]
    private static void Sleep()
    {
        var wait = random.Next(100, 1000);
        Console.WriteLine($"Sleeping for {wait} ms!");
        Thread.Sleep(wait);
    }
    
}


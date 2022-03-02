using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Builds a default IProfiler instance and adds it to the IServiceCollection
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="name">The name of the Profiler</param>
    /// <returns></returns>
    public static IServiceCollection AddDefaultProfiler(this IServiceCollection serviceCollection, string name = "Profiler")
    {
        var loggerFactory = serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<IProfiler>();
        var builder = ProfilerBuilder.Create(name);
//        builder.UseAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        builder.UseAssemblies(Assembly.GetEntryAssembly());

        builder.UseLogger(logger);

        serviceCollection.AddSingleton<IProfiler>(builder.Build());

        return serviceCollection;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using MinimalProfiler.Core.Attributes;
using MinimalProfiler.Core.Enums;
using MinimalProfiler.Core.Internal.Patch;
using MinimalProfiler.Core.Models;

namespace MinimalProfiler.Core.Internal;

/// <summary>
/// Main class of this application which is used to patch methods and execute result handlers
/// </summary>
public class Profiler : IProfiler
{
    public string Name { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPatched { get; private set; }

    private IEnumerable<string> PatchedMethods => methods?.Select(m => m.Method.Name);
    private List<IProfilingResultHandler> resultHandlers;
    private readonly List<PatchMethod> methods;
    private readonly ILogger log;
    private readonly Harmony harmony;

    internal Profiler(string name, ILogger log, IEnumerable<Assembly> assemblies, bool runOnBuild, List<IProfilingResultHandler> resultHandlers)
    {
        Name = name;
        this.resultHandlers = resultHandlers;
        this.log = log;
        harmony = new Harmony(name);
        methods = new List<PatchMethod>();

        if (GlobalProfilingState.Instance.DoesProfilerExists(name))
            throw new Exception($"A profiler with the name '{name}' already exists for this application");

        if (!assemblies.Any())
            log.LogWarning("No assemblies specified, so no profiling can be done");

        foreach (var assembly in assemblies)
            methods.AddRange(assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                .Where(m =>
                {
                    var attr = (IProfilerAttribute) m.GetCustomAttributes()
                        .FirstOrDefault(a => a is IProfilerAttribute);
                    return attr != null && (attr.ProfilerName == Name || string.IsNullOrEmpty(attr.ProfilerName));
                }).Select(m =>
                {
                    var attr = m.GetCustomAttributes().FirstOrDefault(a => a is IProfilerAttribute);
                    return attr is ProfileMeAttribute
                        ? new PatchMethod(m, ProfilingPatchType.Normal)
                        : new PatchMethod(m, ProfilingPatchType.Async);
                }));
        log.LogDebug("Found {Methods} profileable methods in {Count} assemblies", methods.Count, assemblies.Count());

        if (runOnBuild)
        {
            PatchAll();
            IsRunning = true;
        }

        GlobalProfilingState.Instance.RegisterProfiler(this);
        log.LogInformation("Profiler created: {Name}", Name);
    }

    public void AddProfilingResult(ProfilingResult result)
    {
        foreach (var handler in resultHandlers)    
        {
            handler.Handle(result);
        }
    }

    /// <summary>
    /// Starts the profiling and patches all methods if not already done
    /// </summary>
    public void Start()
    {
        log.LogInformation("Starting profiling for {Name}", Name);

        if (IsRunning)
            return;

        if (!IsPatched)
            PatchAll();
        IsRunning = true;
    }

    /// <summary>
    /// Stops profiling
    /// </summary>
    public void Stop()
    {
        log.LogInformation("Stopping profiling for {Name}", Name);

        if (!IsRunning)
            return;
        IsRunning = false;
    }

    /// <summary>
    /// Removes the profiler from the global state and unpatches all methods
    /// </summary>
    public void Remove()
    {
        log.LogInformation("Removing profiler {Name}", Name);
        Stop();
        GlobalProfilingState.Instance.RemoveProfiler(Name);
        //TODO unpatch all methods
        IsPatched = false;
    }

    private void PatchAll()
    {
        var profilerPrefix = AccessTools.Method(typeof(ProfilingMethods), "StartProfiling");
        var profilerPostfix = AccessTools.Method(typeof(ProfilingMethods), "StopProfiling");
        var profilerPostfixAsync = AccessTools.Method(typeof(ProfilingMethods), "StopProfilingAsync");

        int patchedMethods = 0;
        foreach (var info in methods)
        {
            var method = info.Method;
            try
            {
                var postfix = info.PatchType == ProfilingPatchType.Normal ? profilerPostfix : profilerPostfixAsync;
                log.LogDebug("Trying to patch {MethodName} with {PrefixName} and {PostfixName}", method.Name, profilerPrefix.Name, postfix.Name);
                harmony.Patch(info.Method, new HarmonyMethod(profilerPrefix), new HarmonyMethod(postfix));
                patchedMethods++;
            }
            catch (Exception e)
            {
                log.LogError(e,"Failed to patch {MethodName}",method.Name);
            }
        }

        log.LogDebug("Patched {Count} methods", patchedMethods);
        IsPatched = true;
    }
}
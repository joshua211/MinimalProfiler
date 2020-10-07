# MinimalProfiler

WIP Profiler that can track method execution speed by decorating it with one attribute. Required code is added at runtime using 
[Harmony](https://github.com/pardeike/Harmony)


- [MinimalProfiler](#minimalprofiler)
  - [Getting started](#getting-started)
  - [Options](#options)
    - [Log](#log)
    - [Build in loggers](#build-in-loggers)
    - [Format](#format)
  - [Profiling](#profiling)
    - [Measurements](#measurements)
    - [Naming](#naming)
    - [Different Profilers](#different-profilers)
  - [Things to note](#things-to-note)
      - [Async/Threaded](#asyncthreaded)
      - [Complexity](#complexity)


## Getting started

Create an instance of `Profiler` using ``Profiler.Create()`` and give him a list of assemblies to search from.
```
 var profiler = Profiler.Create()
                        .UseAssemblies(typeof(Example).Assembly)
                        .Build();
```

Now decorate any method inside of the referenced Assembly with the ``[ProfileMe]`` attribute.
```
public class Example
{
    [ProfileMe]
    public void DoSomething()
    {
        for (long i = 0; i < int.MaxValue; i++)
        {
            //something
        }
    }
}
```

Thats all, the profiler should now print the execution result to the console once you run the method

` [INF] DoSomething took 45677700 ticks | 567 ms to execute `

## Options
### Log
You can provide your own logger, just create a class that implements `ILog` from 'MinimalProfiler.Core.Logging', and tell your profiler to use it
``
var profiler = Profiler.Create().UseLog(log).Build()
``
### Build in loggers
Currently only a console log is available as the default logger. More integrations are planned (Serilog, ..)
### Format
You can provide your own logging format by using ``ProfilerBuilder.UseFormat(Func<ProfilingResult, string>)``
<br>The default format is '``{DisplayName} took {ticks} ticks | {ms} ms to execute``'

## Profiling
### Measurements
Currently only time measurement is supported
### Naming
By default, the profiler will take the method name to use for logging.
You can change this by providing a ``DisplayName`` parameter to the `[ProfileMe]` attribute.
```
[ProfileMe("DifferentName")]
```
### Different Profilers
You can use different profilers for different Assemblies. 
To do this, give each profiler a unique name and provide your `[ProfileMe]` attribute with the name of the desired profiler.
A profiler will only patch methods that have have his profilername as an attribute argument or none.

```
var profiler = Profiler.Create("uniquename")
  
[ProfileMe(profilerName: "uniquename")]
public void Something()
```

## Things to note
#### Async/Threaded
At this point, async and threaded methods are not supported.
#### Complexity
Currently at least one reflection call is made each time a profiled method is executed. 
Since reflection is slow, this will increase the time used to execute the method by a few ticks.
This is usually not a problem, but keep in mind that the measured time is not 100% accurate.

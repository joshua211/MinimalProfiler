# MinimalProfiler

WIP Profiler that can track method execution speed by decorating it with one attribute. Required code is added at runtime using 
Using [Harmony](https://github.com/pardeike/Harmony)

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
### Format
wip

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

```
var profiler = Profiler.Create("uniquename")
  
[ProfileMe(profilerName: "uniquename")]
public void Something()
```

## Things to note
At this point, asnyc and threaded methods are not supported.

# MinimalProfiler

A minimal, easy to use method profiler for dotnet Core.  
Designed to profile method execution by adding a simple attribute to the method.  
Required code is added at runtime using 
[Harmony](https://github.com/pardeike/Harmony)


- [MinimalProfiler](#minimalprofiler)
  - [Getting started](#getting-started)
    - [Install](#install)
    - [Setup](#setup)
  - [Profiler](#profiler)
    - [Fluid syntax](#fluid-syntax)
    - [Dependency Injection](#dependency-injection)
  - [Options](#options)
    - [Logging](#logging)
    - [Format](#format)
  - [Profiling](#profiling)
    - [Measurements](#measurements)
    - [Async](#async)
    - [Naming](#naming)
    - [Different Profilers](#different-profilers)
  - [Things to note](#things-to-note)
      - [Complexity](#complexity)


## Getting started
### Install
Install from [nuget](https://www.nuget.org/packages/minimalprofiler/1.0.0).  
```
dotnet add package minimalprofiler
```  
or clone from Github 
```
git clone git@github.com:joshua211/MinimalProfiler.git
```
Since Harmony is using binary serializer, you have to enable unsafe binary serialization in the .csproj file
```
  <PropertyGroup>
    <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>
  </PropertyGroup>
```

### Setup
Create an instance of `Profiler` using ``Profiler.Create()`` and specify a list of assemblies to search from.
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

## Profiler
The main instance of this library, which is used to patch methods and log profiling results.  
There two ways to create a Profiler.
### Fluid syntax
Call `Profiler.Create()` to get a new instance of `ProfilerBuilder` and build your own Profiler. Default behavior is to patch all methods and start profiling after `.Build(bool run = true)` is called.
### Dependency Injection
You can add the profiler to the `IServiceCollection` and manually call `.Start()` to patch and start profiling.
```
services.AddSingleton<Profiler>();
```
This profiler will use the default log format and search in the calling assembly for methods with the `[ProfileMe]` attribute.
## Options
### Logging
If no logger is specified, the profiler will log to the console. You can use any logger that implements the `ILogger` interface by using ``.UseLog(ILogger logger)`` on the `ProfilerBuilder`.
### Format
You can provide your own logging format by using ``ProfilerBuilder.UseFormat(Func<ProfilingResult, string>)``
<br>The default format is '``{DisplayName} took {ticks} ticks | {ms} ms to execute``'

## Profiling
### Measurements
Currently only time measurement is supported
### Async
To profile an Async method, use the ``[ProfileMeAsync]`` Attribute. All parameters work the same with the async version.  
The target method has to return a **Task**  
### Naming
By default, the profiler will take the method name to use for logging.
You can change this by providing a ``DisplayName`` parameter to the `[ProfileMe]` attribute.
```
[ProfileMe("DifferentName")]
```
### Different Profilers
You can use different profilers in the same project. 
To do this, give each profiler a unique name and provide your `[ProfileMe]` attribute with the `ProfilerName` parameter.
A profiler will only try to patch methods that either have no `ProfilerName` parameter or his own name.

```
var profiler = Profiler.Create("uniquename")
  
[ProfileMe(profilerName: "uniquename")]
public void Something()
```

## Things to note
#### Complexity
Currently at least one reflection call is made each time a profiled method is executed. 
Since reflection is slow, this will increase the time used to execute the method by a few ticks.
This is usually not a problem, but keep in mind that the measured time is not 100% accurate.

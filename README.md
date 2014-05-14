SharpLog
========

NuGet:

> Install-Package SharpLog 

What
---

A simple high-peformance portable logging framework for .NET

It follows simple convention over configuration, and works with PCL.

- Two interfaces **ILogger**, and **ICompositeLogger** (which itself is simply an ILogger)
- CompositeLogger simply takes in any number of ILoggers as targets.
- Numerous abstract classes to quickly implement any type of new Loggers.

Why
---
   
Loggers like NLog, log4Net are great! In fact, if you're already happy with it, stay with it. But they don't work with PCL (as of writing this section), and they bring a lot of complexity to logging. 

SharpLog focuses on being extremely simple, while providing everything that a framework like NLog provides. So, you don't really have to spend time learning the framework. Just get straight on to work, and makes it very easy to find out if something does wrong. 

Also, SharpLog focuses only on high-performance components. For example, the LogManager does not have a GetCurrentClassLogger like NLog. Since it uses a lot of reflection underneath, a better approach of either having your own key-based logger with the name, or using a DI like SimpleInjector to automatically inject based on the context is recommended.

If you find something lacking, just use one of the abstract classes as starting point, or go directly to the core and implement your own ILogger. 

And feel free to contribute tid-bits. :) 

I'll keep adding more loggers as I need them, including Bufferred, File-based, memory-based, network-based loggers.

The Cooler Bits
---

- All loggers have **asynchrony right into its roots**. -> Logger.InfoAsync, Logger.ErrorAsync, etc.

- Debug, and Trace functions are slightly special. They have an extra overload, that even **takes Funcs to offer deffered execution**. :) .. Oh, if you want to write your own logger, you won't have to do a thing. Its all already wired up for you. 

    LogManager.Logger.Debug((ex) => ex.StackTrace, exception);

- When I say **high-performance, I'm very serious about it**. There are **no multi-level nested checking for log levels, and if they are enabled**. There is just one check that happens. And as you, see above, there's no other logger that I know of, offers deffered execution using Funcs for Debug, and Trace. So, you never incur a performance hit of unwinding through your stackTrace, even if you have logging and or Trace disabled.


Example
---

```C#
// Automatically attached to Log Manager, and also sets it as the default logger to be used
// from LogManager.Log

var l = LogManager.CreateLogger<ColoredConsoleLogger>();

// Default level is Trace, on Debug builds, and Warn+Trace on Release.
LogManager.Logger.Level = LogLevel.Trace;

// Calling the default logger:

LogManager.Log("Hello");
LogManager.LogAsync("Log the same async.");
LogManager.Log(LogLevel.Critical, "This is critical!");

// Alternate ways:

// Exactly the same as above. This is how the above is implemented internally.

LogManager.Logger.Critical("LM: Critical");
LogManager.Logger.Error("LM: oops");
LogManager.Logger.Warn("LM: warning, warning!!");
LogManager.Logger.Info("LM: Hi!");

LogManager.Logger.Debug("LM: Some debug data.");
LogManager.Logger.Trace("LM: Yello");

l.Critical("DIRECT: This should work as well.");

var newLogger = new ConsoleLogger();
newLogger.Level = LogLevel.Trace;
newLogger.IsEnabled = true;
newLogger.Info("DIRECT: SayHi!");

LogManager.AttachLogger(newLogger, "new");

var oldConsoleLogger = LogManager.GetLogger("new");
oldConsoleLogger.Trace("INDIRECT: TRACE Info");

LogManager.DisableAll();

oldConsoleLogger.Info("INDIRECT: won't be printed..");

LogManager.EnableAll();

var cl = new CompositeLogger();
cl.Level = LogLevel.Trace;

cl.AttachTarget(oldConsoleLogger);
cl.Critical("COMPOSITE: hi from composite!");
cl.Info("COMPOSITE: Info");

oldConsoleLogger.Info("Bye bye!");

```


The API
---

```C#

 	public interface ILogger : IDisposable
    {
        bool IsEnabled { get; set; }
        bool IsTracingEnabled { get; set; }
        bool IsSynchronized { get; }
        LogLevel Level { get; set; }

        void Critical(string text, [CallerMemberName] string callerName = null);
        void Error(string text, [CallerMemberName] string callerName = null);
        void Warn(string text, [CallerMemberName] string callerName = null);
        void Info(string text, [CallerMemberName] string callerName = null);
        void Debug(string text, [CallerMemberName] string callerName = null);
        void Debug(Func<object, string> textFunc, object state = null, [CallerMemberName] string callerName = null);
        void Trace(string text, [CallerMemberName] string callerName = null);
        void Trace(Func<object, string> textFunc, object state = null, [CallerMemberName] string callerName = null);

        Task CriticalAsync(string text, [CallerMemberName] string callerName = null);
        Task ErrorAsync(string text, [CallerMemberName] string callerName = null);
        Task WarnAsync(string text, [CallerMemberName] string callerName = null);
        Task InfoAsync(string text, [CallerMemberName] string callerName = null);
        Task DebugAsync(string text, [CallerMemberName] string callerName = null);
        Task DebugAsync(Func<object, string> textFunc, object state = null, [CallerMemberName] string callerName = null);
        Task TraceAsync(string text, [CallerMemberName] string callerName = null);
        Task TraceAsync(Func<object, string> textFunc, object state = null, [CallerMemberName] string callerName = null);

    }
```

Developers, fret not, you'd almost never have to implement the whole thing yourself. 

There're abstract classes for that. **90% of the time, you'd just end up implement one Execute method, and an optional ExecuteAsync method**. Everything else is already wired-up for you and ready to go.

---

### Fun and useful ideas:

- XAML-based overlay logger (So, it works on all platforms)
- Network logger and a receiver GUI over network.
- OpenGL based overlay logger

Cheers.

PVL

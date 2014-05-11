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
   
Loggers like NLog are great! In fact, if you're already happy with it, stay with it. But they don't work with PCL (as of writing this section), and they bring a lot of complexity to logging. 

SharpLog focuses on being extremely simple, while providing everything that a framework like NLog provides. So, you don't really have to spend time learning the framework. Just get straight on to work, and makes it very easy to find out if something does wrong. 

Also, SharpLog focuses only on high-performance components. For example, the LogManager does not have a GetCurrentClassLogger like NLog. Since it uses a lot of reflection underneath, this practice is not recommended. As an alternative, create your own key-based logger with the name. 

If you find something lacking, just use one of the abstract classes as starting point, or go directly to the core and implement your own ILogger. 

And feel free to contribute tid-bits. :) 

I'll keep adding more loggers as I need them, including Bufferred, File-based, memory-based, network-based loggers.

---

### Fun and useful ideas:

- XAML-based overlay logger (So, it works on all platforms)
- Network logger and a receiver GUI over network.
- OpenGL based overlay logger

Cheers.

PVL

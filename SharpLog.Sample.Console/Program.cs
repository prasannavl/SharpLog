using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog.Sample.Console
{
    using System.Reflection;
    using SharpLog.Desktop;

    class Program
    {
        static void Main(string[] args)
        {
            // Automatically attached to Log Manager, and also sets it as the default logger to be used
            // from LogManager.Log

            var l = LogManager.CreateLogger<ConsoleLogger>();

            // Default level is Trace, on Debug builds, and Warn+Trace on Release.
            LogManager.Log.Level = LogLevel.Trace;

            LogManager.Log.Critical("LM: Critical");
            LogManager.Log.Error("LM: oops");
            LogManager.Log.Warn("LM: warning, warning!!");
            LogManager.Log.Info("LM: Hi!");

            LogManager.Log.Debug("LM: deeeeug");
            LogManager.Log.Trace("LM: Yollo");

            l.Critical("DIRECT: This should work as well.");

            var newLogger = new ConsoleLogger();
            newLogger.Level = LogLevel.Trace;
            newLogger.IsEnabled = true;
            newLogger.Info("DIRECT: SayHi!");

            LogManager.AttachLogger(newLogger, "new");

            var again = LogManager.GetLogger("new");
            again.Trace("INDIRECT: TRACE Info");

            LogManager.DisableAll();

            again.Info("INDIRECT: won't be printed..");

            LogManager.EnableAll();

            var cl = new CompositeLogger();
            cl.Level = LogLevel.Trace;

            cl.AttachTarget(again);
            cl.Critical("COMPOSITE: hi from composite!");
            cl.Info("COMPOSITE: Info");

            again.Info("Bye bye!");
        }
    }
}

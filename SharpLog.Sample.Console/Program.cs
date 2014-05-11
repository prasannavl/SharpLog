using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog.Sample.Console
{
    using SharpLog.Desktop;

    class Program
    {
        static void Main(string[] args)
        {
            // Automatically attached to Log Manager, and also sets it as the default logger to be used
            // from LogManager.Log

            var l = LogManager.CreateLogger<ConsoleLogger>();

            // Default level is Trace, on Debug builds, and Warn+Trace on Release.
            LogManager.Log.Level = LogLevel.Error;

            // Logging is disabled by default on release builds.
            LogManager.Log.IsEnabled = true;


            LogManager.Log.Level = LogLevel.Info;

            LogManager.Log.Critical("Critical");
            LogManager.Log.Error("oops");
            LogManager.Log.Warn("warning, warning!!");
            LogManager.Log.Info("Hi!");

            LogManager.Log.Debug("deeeeug");
            LogManager.Log.Trace("Yollo");

            l.Critical("This should work as well.");

            var newLogger = new ConsoleLogger();
            newLogger.Level = LogLevel.Trace;
            newLogger.IsEnabled = true;
            newLogger.Info("SayHi!");

            LogManager.AttachLogger(newLogger, "new");

            var again = LogManager.GetLogger("new");
            again.Trace("TRACE Info");

            LogManager.DisableAll();

            again.Info("won't be printed..");

            LogManager.EnableAll();

            again.Info("Bye bye!");
        }
    }
}

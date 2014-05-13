// Author: Prasanna V. Loganathar
// Project: SharpLog.Sample.Console
// 
// Copyright 2014 Launchark. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//  
// Created: 12:24 AM 12-05-2014

namespace SharpLog.Sample.Console
{
    using SharpLog.Desktop;

    internal class Program
    {
        private static void Main(string[] args)
        {
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
        }
    }
}
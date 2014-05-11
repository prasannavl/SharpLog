// Author: Prasanna V. Loganathar
// Project: SharpLog
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
// Created: 2:43 AM 11-05-2014

namespace SharpLog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using SharpLog.PortablilityScaffolds;

    public static class LogManager
    {
        private static readonly IConcurrentDictionary<string, ILogger> Loggers;
        public static readonly ILogger NullLogger = new NullLogger();
        private static ILogger log;

        static LogManager()
        {
            Loggers = Global.Services.GetInstance<IConcurrentDictionary<string, ILogger>>();
        }

        public static ILogger Log
        {
            get
            {
                if (log == null)
                {
                    Interlocked.CompareExchange(ref log, NullLogger, null);
                }
                return log;
            }
            set
            {
                SetDefaultLogger(value);
            }
        }

        public static void EnableAll(bool enableTrace = true)
        {
            foreach (var logger in Loggers)
            {
                logger.Value.IsEnabled = true;
                if (enableTrace) logger.Value.IsTracingEnabled = true;
            }
        }

        public static void DisableAll(bool disableTrace = true)
        {
            foreach (var logger in Loggers)
            {
                logger.Value.IsEnabled = false;
                if (disableTrace) logger.Value.IsTracingEnabled = false;
            }
        }

        private static void SetDefaultLogger(ILogger logger)
        {
            log = logger;
        }

        public static bool AttachLogger(ILogger logger, string name = null)
        {
            if (name == null)
            {
                name = Guid.NewGuid().ToString();
            }
            if (log == null)
            {
                SetDefaultLogger(logger);
            }
            return Loggers.TryAdd(name, logger);
        }

        public static void DetachLogger(string name, bool dispose = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (Loggers.ContainsKey(name))
            {
                ILogger value = null;
                if (Loggers.TryGetValue(name, out value))
                {
                    Loggers.Remove(name);

                    if (dispose)
                    {
                        value.Dispose();
                    }
                }
            }
        }

        public static void DetachLogger(ILogger logger, bool dispose = true)
        {
            var currentLog = log;
            foreach (var target in Loggers.Where(x => x.Value == logger).ToArray())
            {
                if (currentLog == target.Value) log = NullLogger;

                Loggers.Remove(target.Key);
                if (dispose)
                {
                    target.Value.Dispose();
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, ILogger>> GetAllLoggers()
        {
            return Loggers;
        }

        public static ILogger GetLogger(string name)
        {
            ILogger logger = null;
            if (Loggers.ContainsKey(name))
            {
                Loggers.TryGetValue(name, out logger);
            }

            return logger;
        }

        public static void ClearAllLoggers(bool dispose = true)
        {
            if (dispose)
            {
                foreach (var logger in Loggers.ToArray())
                {
                    Loggers.Remove(logger);
                    logger.Value.Dispose();
                }
            }
            else
            {
                Loggers.Clear();
            }

            Log = NullLogger;
        }

        public static T CreateLogger<T>() where T : ILogger
        {
            var logger = (T)Activator.CreateInstance(typeof(T));
            AttachLogger(logger);
            return logger;
        }

        public static T CreateLogger<T>(params object[] arguments) where T : ILogger
        {
            var logger = (T)Activator.CreateInstance(typeof(T), arguments);
            AttachLogger(logger);
            return logger;
        }
    }
}
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

    using SharpLog.PortableScaffolds;

    public class LogManager
    {
        private readonly IConcurrentDictionary<string, ILogger> loggers;
        private ILogger log;

        public LogManager()
        {
            loggers = Global.Services.GetInstance<IConcurrentDictionary<string, ILogger>>();
        }

        public ILogger Log
        {
            get
            {
                if (log == null)
                {
                    Interlocked.CompareExchange(ref log, new NullLogger(), null);
                }
                return log;
            }
            set
            {
                SetDefaultLogger(value);
            }
        }

        private void SetDefaultLogger(ILogger logger)
        {
            log = logger;
        }

        public bool AttachLogger(ILogger logger, string name = null)
        {
            if (name == null)
            {
                name = Guid.NewGuid().ToString();
            }
            if (log == null)
            {
                SetDefaultLogger(logger);
            }
            return loggers.TryAdd(name, logger);
        }

        public void DetachLogger(string name, bool dispose = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (loggers.ContainsKey(name))
            {
                ILogger value = null;
                if (loggers.TryGetValue(name, out value))
                {
                    loggers.Remove(name);

                    if (dispose)
                    {
                        value.Dispose();
                    }
                }
            }
        }

        public void DetachLogger(ILogger logger, bool dispose = true)
        {
            foreach (var target in loggers.Where(x => x.Value == logger).ToArray())
            {
                loggers.Remove(target.Key);
                if (dispose)
                {
                    target.Value.Dispose();
                }
            }
        }

        public virtual IEnumerable<KeyValuePair<string, ILogger>> GetAllLoggers()
        {
            return loggers;
        }

        public virtual ILogger GetLogger(string name)
        {
            ILogger logger = null;
            if (loggers.ContainsKey(name))
            {
                loggers.TryGetValue(name, out logger);
            }

            return logger;
        }

        public void ClearAllLoggers(bool dispose = true)
        {
            if (dispose)
            {
                foreach (var logger in loggers.ToArray())
                {
                    loggers.Remove(logger);
                    logger.Value.Dispose();
                }
            }
            else
            {
                loggers.Clear();
            }
        }

        public T CreateLogger<T>() where T : ILogger
        {
            var logger = (T)Activator.CreateInstance(typeof(T));
            AttachLogger(logger);
            return logger;
        }

        public T CreateLogger<T>(params object[] arguments) where T : ILogger
        {
            var logger = (T)Activator.CreateInstance(typeof(T), arguments);
            AttachLogger(logger);
            return logger;
        }
    }
}
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
// Created: 7:15 PM 07-05-2014

namespace SharpLog
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public abstract class LoggerBase : ILogger
    {
        protected LoggerBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            SetDefaultLevel();
        }

        protected LoggerBase(LogLevel level)
        {
            Level = level;
        }

        public LogLevelState LogLevelState { get; protected set; }

        public virtual bool IsEnabled
        {
            get
            {
                return LogLevelState > LogLevelState.Enabled;
            }

            set
            {
                if (value)
                {
                    LogLevelState |= LogLevelState.Enabled;
                }
                else
                {
                    LogLevelState &= ~LogLevelState.Enabled;
                }
            }
        }

        public virtual bool IsTracingEnabled
        {
            get
            {
                return LogLevelState.HasFlag(LogLevelState.Trace);
            }

            set
            {
                if (value)
                {
                    LogLevelState |= LogLevelState.Trace;
                }
                else
                {
                    LogLevelState &= ~LogLevelState.Trace;
                }
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public LogLevel Level
        {
            get
            {
                // Clear the enabled flag to get the actual LogLevel.
                return (LogLevel)(LogLevelState & ~LogLevelState.Enabled);
            }

            set
            {
                if (!Enum.IsDefined(typeof(LogLevel), value))
                {
                    throw new ArgumentException("Invalid LogLevel option.");
                }

                // Make sure, the previous state of IsEnabled is restored, 
                // while restoring the log level.
                if (IsEnabled)
                {
                    LogLevelState = (LogLevelState)value | LogLevelState.Enabled;
                }
                else
                {
                    LogLevelState = (LogLevelState)value;
                }
            }
        }

        public abstract void Critical(string text, string callerName = null);
        public abstract void Error(string text, string callerName = null);
        public abstract void Warn(string text, string callerName = null);
        public abstract void Info(string text, string callerName = null);
        public abstract void Debug(string text, string callerName = null);
        public abstract void Trace(string text, string callerName = null);

        public abstract Task CriticalAsync(string text, string callerName = null);
        public abstract Task WarnAsync(string text, string callerName = null);
        public abstract Task InfoAsync(string text, string callerName = null);
        public abstract Task DebugAsync(string text, string callerName = null);
        public abstract Task TraceAsync(string text, string callerName = null);
        public abstract Task ErrorAsync(string text, string callerName = null);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void SetDefaultLevel()
        {
            if (Debugger.IsAttached)
            {
                // Set to LogLevel.Trace (full logging with traces) state directly using LogLevelState.
                LogLevelState = LogLevelState.Trace | LogLevelState.Debug;
            }
            else
            {
                LogLevelState = LogLevelState.Warn;
            }
        }

        protected abstract void Dispose(bool disposing);
    }
}
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
// Created: 2:47 AM 11-05-2014

namespace SharpLog
{
    using System;
    using System.Threading.Tasks;

    public class NullLogger : ILogger
    {
        public bool IsEnabled
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public bool IsTracingEnabled
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public LogLevel Level
        {
            get
            {
                return LogLevel.None;
            }

            set
            {
            }
        }

        public void Critical(string text, string callerName = null)
        {
        }

        public void Error(string text, string callerName = null)
        {
        }

        public void Warn(string text, string callerName = null)
        {
        }

        public void Info(string text, string callerName = null)
        {
        }

        public void Debug(string text, string callerName = null)
        {
        }

        public void Debug(Func<object, string> textFunc, object state = null, string callerName = null)
        {
        }

        public void Trace(string text, string callerName = null)
        {
        }

        public void Trace(Func<object, string> textFunc, object state = null, string callerName = null)
        {
        }

        public Task CriticalAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task ErrorAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task WarnAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task InfoAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task DebugAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task DebugAsync(Func<object, string> textFunc, object state = null, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task TraceAsync(string text, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public Task TraceAsync(Func<object, string> textFunc, object state = null, string callerName = null)
        {
            return Helpers.CompletedTask;
        }

        public void Dispose()
        {
        }

        protected void Dispose(bool disposing)
        {
        }
    }
}
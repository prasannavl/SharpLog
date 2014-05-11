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
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class Logger : LoggerBase
    {
        protected Logger()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            IsEnabled = true;
        }

        protected abstract void Execute(LogLevel level, string text, string callerName);
        protected abstract void Execute(Exception ex, string callerName);
        protected abstract Task ExecuteAsync(LogLevel level, string text, string callerName);
        protected abstract Task ExecuteAsync(Exception ex, string callerName);

        public override void Critical(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                Execute(LogLevel.Critical, text, callerName);
            }
        }

        public override void Error(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                Execute(LogLevel.Error, text, callerName);
            }
        }

        public override void Warn(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                Execute(LogLevel.Warn, text, callerName);
            }
        }

        public override void Info(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                Execute(LogLevel.Info, text, callerName);
            }
        }

        public override void Debug(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                Execute(LogLevel.Debug, text, callerName);
            }
        }

        public override void Trace(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                Execute(LogLevel.Trace, text, callerName);
            }
        }

        public override Task CriticalAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                ExecuteAsync(LogLevel.Critical, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task ErrorAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                return ExecuteAsync(LogLevel.Error, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task WarnAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                return ExecuteAsync(LogLevel.Warn, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task InfoAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                return ExecuteAsync(LogLevel.Info, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task DebugAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                return ExecuteAsync(LogLevel.Debug, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task TraceAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                return ExecuteAsync(LogLevel.Trace, text, callerName);
            }
            return Helpers.CompletedTask;
        }
    }
}
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
// Created: 7:17 PM 07-05-2014

namespace SharpLog
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class FormattableLogger : LoggerBase
    {
        public abstract string Format(string text, LogLevel level, string callerName);

        public override void Critical(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                text = Format(text, LogLevel.Critical, callerName);
                Execute(LogLevel.Critical, text, callerName);
            }
        }

        public override void Error(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                text = Format(text, LogLevel.Error, callerName);
                Execute(LogLevel.Error, text, callerName);
            }
        }

        public override void Warn(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                text = Format(text, LogLevel.Warn, callerName);
                Execute(LogLevel.Warn, text, callerName);
            }
        }

        public override void Info(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                text = Format(text, LogLevel.Info, callerName);
                Execute(LogLevel.Info, text, callerName);
            }
        }

        public override void Debug(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                text = Format(text, LogLevel.Debug, callerName);
                Execute(LogLevel.Debug, text, callerName);
            }
        }

        public override void Debug<T>(Func<T, string> textFunc, T state, string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                var text = Format(textFunc(state), LogLevel.Debug, callerName);
                Execute(LogLevel.Debug, text, callerName);
            }
        }

        public override void Trace(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                text = Format(text, LogLevel.Trace, callerName);
                Execute(LogLevel.Trace, text, callerName);
            }
        }

        public override void Trace<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                var text = Format(textFunc(state), LogLevel.Trace, callerName);
                Execute(LogLevel.Trace, text, callerName);
            }
        }

        public override Task CriticalAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                text = Format(text, LogLevel.Critical, callerName);
                return ExecuteAsync(LogLevel.Critical, text, callerName);
            }

            return Helpers.CompletedTask;
        }

        public override Task WarnAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                text = Format(text, LogLevel.Warn, callerName);
                return ExecuteAsync(LogLevel.Warn, text, callerName);
            }

            return Helpers.CompletedTask;
        }

        public override Task InfoAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                text = Format(text, LogLevel.Info, callerName);
                return ExecuteAsync(LogLevel.Info, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task DebugAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                text = Format(text, LogLevel.Debug, callerName);
                return ExecuteAsync(LogLevel.Debug, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task DebugAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                var text = Format(textFunc(state), LogLevel.Debug, callerName);
                return ExecuteAsync(LogLevel.Debug, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task TraceAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                text = Format(text, LogLevel.Trace, callerName);
                return ExecuteAsync(LogLevel.Trace, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task TraceAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                var text = Format(textFunc(state), LogLevel.Trace, callerName);
                return ExecuteAsync(LogLevel.Trace, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        public override Task ErrorAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                text = Format(text, LogLevel.Error, callerName);
                return ExecuteAsync(LogLevel.Error, text, callerName);
            }
            return Helpers.CompletedTask;
        }

        protected abstract void Execute(LogLevel level, string text, string callerName);
        protected abstract Task ExecuteAsync(LogLevel level, string text, string callerName);
    }
}
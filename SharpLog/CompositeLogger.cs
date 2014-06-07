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
// Created: 7:37 PM 07-05-2014

namespace SharpLog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using SharpLog.PortabilityScaffolds;

    public interface ICompositeLogger : ILogger
    {
        bool AttachTarget(ILogger logger, string name = null);
        void DetachTarget(string name, bool dispose = true);
        void DetachTarget(ILogger logger, bool dispose = true);
        void ClearTargets();
        IEnumerable<KeyValuePair<string, ILogger>> GetAllTargets();
        ILogger GetTarget(string name);
    }

    public class CompositeLogger : LoggerBase, ICompositeLogger
    {
        public CompositeLogger()
        {
            Targets = Global.Container.GetInstance<IConcurrentDictionary<string, ILogger>>();
            IsEnabled = true;
        }

        private IConcurrentDictionary<string, ILogger> Targets { get; set; }

        public override bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public virtual bool AttachTarget(ILogger logger, string name = null)
        {
            if (name == null)
            {
                name = Guid.NewGuid().ToString();
            }
            return Targets.TryAdd(name, logger);
        }

        public virtual void DetachTarget(string name, bool dispose = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (Targets.ContainsKey(name))
            {
                ILogger value = null;
                if (Targets.TryGetValue(name, out value))
                {
                    Targets.Remove(name);

                    if (dispose)
                    {
                        value.Dispose();
                    }
                }
            }
        }

        public virtual void DetachTarget(ILogger logger, bool dispose = true)
        {
            foreach (var target in Targets.Where(x => x.Value == logger).ToList())
            {
                Targets.Remove(target.Key);
                if (dispose)
                {
                    target.Value.Dispose();
                }
            }
        }

        public virtual void ClearTargets()
        {
            Targets.Clear();
        }

        public override void Critical(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                foreach (var target in Targets)
                {
                    target.Value.Critical(text, callerName);
                }
            }
        }

        public override void Error(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                foreach (var target in Targets)
                {
                    target.Value.Error(text, callerName);
                }
            }
        }

        public override void Warn(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                foreach (var target in Targets)
                {
                    target.Value.Warn(text, callerName);
                }
            }
        }

        public override void Info(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                foreach (var target in Targets)
                {
                    target.Value.Info(text, callerName);
                }
            }
        }

        public override void Debug(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                foreach (var target in Targets)
                {
                    target.Value.Debug(text, callerName);
                }
            }
        }

        public override void Trace(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                foreach (var target in Targets)
                {
                    target.Value.Trace(text, callerName);
                }
            }
        }

        public override void Trace<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                var text = textFunc(state);
                foreach (var target in Targets)
                {
                    target.Value.Trace(text, callerName);
                }
            }
        }

        public override async Task CriticalAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledCriticalLowerThreshold)
            {
                await Task.WhenAll(Targets.Select(x => x.Value.CriticalAsync(text, callerName)));
            }
        }

        public override async Task ErrorAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledErrorLowerThreshold)
            {
                await Task.WhenAll(Targets.Select(x => x.Value.ErrorAsync(text, callerName)));
            }
        }

        public override async Task WarnAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledWarnLowerThreshold)
            {
                await Task.WhenAll(Targets.Select(x => x.Value.WarnAsync(text, callerName)));
            }
        }

        public override async Task InfoAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledInfoLowerThreshold)
            {
                await Task.WhenAll(Targets.Select(x => x.Value.InfoAsync(text, callerName)));
            }
        }

        public override async Task DebugAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                await Task.WhenAll(Targets.Select(x => x.Value.DebugAsync(text, callerName)));
            }
        }

        public override async Task TraceAsync(string text, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                await Task.WhenAll(Targets.Select(x => x.Value.TraceAsync(text, callerName)));
            }
        }

        public virtual IEnumerable<KeyValuePair<string, ILogger>> GetAllTargets()
        {
            return Targets;
        }

        public virtual ILogger GetTarget(string name)
        {
            ILogger logger = null;
            if (Targets.ContainsKey(name))
            {
                Targets.TryGetValue(name, out logger);
            }

            return logger;
        }

        public override void Debug<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                var text = textFunc(state);
                foreach (var target in Targets)
                {
                    target.Value.Debug(text, callerName);
                }
            }
        }

        public override async Task DebugAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState > LogLevelState.EnabledDebugLowerThreshold)
            {
                var text = textFunc(state);
                await Task.WhenAll(Targets.Select(x => x.Value.DebugAsync(text, callerName)));
            }
        }

        public override async Task TraceAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null)
        {
            if (LogLevelState.HasFlag(LogLevelState.Trace))
            {
                var text = textFunc(state);
                await Task.WhenAll(Targets.Select(x => x.Value.TraceAsync(text, callerName)));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var target in Targets)
                {
                    target.Value.Dispose();
                }
            }
        }
    }
}
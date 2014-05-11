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
// Created: 11:09 PM 10-05-2014

namespace SharpLog
{
    using System;
    using System.Collections.Generic;

    using SharpLog.PortableScaffolds;

    // INCOMPLETE - Not Included in project.
    // TODO: Find the most efficient way to store cateogorically.
    public class InMemoryLogger : SynchronousLogger
    {
        private readonly IConcurrentDictionary<LogLevel, IConcurrentDictionary<string, List<string>>> messageStore;

        public InMemoryLogger()
        {
            messageStore =
                Global.Services
                    .GetInstance<IConcurrentDictionary<LogLevel, IConcurrentDictionary<string, List<string>>>>();
            foreach (var level in
                new[]
                    { LogLevel.Critical, LogLevel.Debug, LogLevel.Error, LogLevel.Info, LogLevel.Trace, LogLevel.Warn })
            {
                messageStore[level] = Global.Services.GetInstance<IConcurrentDictionary<string, List<string>>>();
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            
        }

        protected override void Execute(LogLevel level, string text, string callerName)
        {
            var store = messageStore[level];
            if (store.ContainsKey(callerName))
            {
                var list = store[callerName];
                list.Add(text);
            }
            else
            {
                store.TryAdd(callerName, new List<string> { text });
            }
        }

        protected override void Execute(Exception ex, string callerName)
        {
            var store = messageStore[LogLevel.Error];
            if (store.ContainsKey(callerName))
            {
                store[callerName].Add(ex.Message);
            }
            else
            {
                store.TryAdd(callerName, new List<string> { ex.Message });
            }
        }
    }
}
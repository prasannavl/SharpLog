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
// Created: 2:18 AM 10-05-2014

namespace SharpLog
{
    using System;

    public class DebugLogger : SynchronousFormattableLogger
    {
        public override string Format(string text, LogLevel level, string callerName)
        {
            return string.Format("{0}: {1} -> {2}", level.ToString().ToUpperInvariant(), callerName, text);
        }

        protected override void Execute(LogLevel level, string text, string callerName)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
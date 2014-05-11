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
// Created: 3:24 AM 10-05-2014

namespace SharpLog
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class SynchronousFormattableLogger : FormattableLogger
    {
        protected override Task ExecuteAsync(LogLevel level, string text, string callerName)
        {
            return Task.Run(() => Execute(level, text, callerName));
        }
    }
}
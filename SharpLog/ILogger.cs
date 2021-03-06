﻿// Author: Prasanna V. Loganathar
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
// Created: 3:05 AM 21-05-2014

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharpLog
{
    public interface ILogger : IDisposable
    {
        bool IsEnabled { get; set; }
        bool IsTracingEnabled { get; set; }
        bool IsSynchronized { get; }
        LogLevel Level { get; set; }

        void Critical(string text, [CallerMemberName] string callerName = null);
        void Error(string text, [CallerMemberName] string callerName = null);
        void Warn(string text, [CallerMemberName] string callerName = null);
        void Info(string text, [CallerMemberName] string callerName = null);
        void Debug(string text, [CallerMemberName] string callerName = null);
        void Debug<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null);

        void Trace(string text, [CallerMemberName] string callerName = null);
        void Trace<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null);

        Task CriticalAsync(string text, [CallerMemberName] string callerName = null);
        Task ErrorAsync(string text, [CallerMemberName] string callerName = null);
        Task WarnAsync(string text, [CallerMemberName] string callerName = null);
        Task InfoAsync(string text, [CallerMemberName] string callerName = null);
        Task DebugAsync(string text, [CallerMemberName] string callerName = null);
        Task DebugAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null);
        Task TraceAsync(string text, [CallerMemberName] string callerName = null);
        Task TraceAsync<T>(Func<T, string> textFunc, T state, [CallerMemberName] string callerName = null);
    }
}
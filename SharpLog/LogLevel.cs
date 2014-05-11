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
// Created: 7:13 PM 07-05-2014

namespace SharpLog
{
    using System;

    public enum LogLevel
    {
        None = LogLevelState.None,
        Critical = LogLevelState.Critical,
        Error = LogLevelState.Error,
        Warn = LogLevelState.Warn,
        Info = LogLevelState.Info,
        Debug = LogLevelState.Debug,
        Trace = LogLevelState.Trace | Debug,

        TraceOnly = LogLevelState.Trace,

        CriticalAndTrace = Critical | Trace,
        ErrorAndTrace = Error | Trace,
        WarnAndTrace = Warn | Trace,
        InfoAndTrace = Info | Trace,
    }

    [Flags]
    public enum LogLevelState : byte
    {
        None = 0,
        Trace = 0x01,
        Critical = 0x02,
        Error = 0x04,
        Warn = 0x08,
        Info = 0x10,
        Debug = 0x20,

        Enabled = 0x80,

        CriticalAndTrace = Critical | Trace,
        ErrorAndTrace = Error | Trace,
        WarnAndTrace = Warn | Trace,
        InfoAndTrace = Info | Trace,

        EnabledCriticalLowerThreshold = Enabled | Trace,
        EnabledErrorLowerThreshold = Enabled | Critical,
        EnabledWarnLowerThreshold = Enabled | Error,
        EnabledInfoLowerThreshold = Enabled | Info,
        EnabledDebugLowerThreshold = Enabled | Info,
    }
}
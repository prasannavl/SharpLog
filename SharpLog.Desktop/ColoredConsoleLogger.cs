// Author: Prasanna V. Loganathar
// Project: SharpLog.Desktop
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
// Created: 4:46 PM 11-05-2014

namespace SharpLog.Desktop
{
    using System;

    public class ColoredConsoleLogger : SynchronousFormattableLogger
    {
        private ConsoleColor previousColorState;
        private ConsoleColor criticalColor = ConsoleColor.Red;
        private ConsoleColor errorColor = ConsoleColor.Yellow;
        private ConsoleColor warnColor = ConsoleColor.Cyan;
        private ConsoleColor infoColor = ConsoleColor.Gray;
        private ConsoleColor debugColor = ConsoleColor.DarkGray;
        private ConsoleColor traceColor = ConsoleColor.White;

        public override bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public ConsoleColor ErrorColor
        {
            get
            {
                return errorColor;
            }
            set
            {
                errorColor = value;
            }
        }

        public ConsoleColor CriticalColor
        {
            get
            {
                return criticalColor;
            }
            set
            {
                criticalColor = value;
            }
        }

        public ConsoleColor WarnColor
        {
            get
            {
                return warnColor;
            }
            set
            {
                warnColor = value;
            }
        }

        public ConsoleColor InfoColor
        {
            get
            {
                return infoColor;
            }
            set
            {
                infoColor = value;
            }
        }

        public ConsoleColor DebugColor
        {
            get
            {
                return debugColor;
            }
            set
            {
                debugColor = value;
            }
        }

        public ConsoleColor TraceColor
        {
            get
            {
                return traceColor;
            }
            set
            {
                traceColor = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
        }

        public void SetGlobalTextColor(ConsoleColor color)
        {
            ErrorColor = CriticalColor = TraceColor = WarnColor = InfoColor = DebugColor = color;
        }

        public override string Format(string text, LogLevel level, string callerName)
        {
            return string.Format("{0}: {1} -> {2}", level.ToString().ToUpperInvariant(), callerName, text);
        }

        protected override void Execute(LogLevel level, string text, string callerName)
        {
            lock (Console.Out)
            {
                previousColorState = Console.ForegroundColor;
                Console.ForegroundColor = GetColor(level);
                Console.WriteLine(text);
                Console.ForegroundColor = previousColorState;
            }
        }

        private ConsoleColor GetColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    return ErrorColor;
                case LogLevel.Critical:
                    return CriticalColor;
                case LogLevel.Debug:
                    return DebugColor;
                case LogLevel.Trace:
                    return TraceColor;
                case LogLevel.Warn:
                    return WarnColor;
                default:
                    return InfoColor;
            }
        }
    }
}
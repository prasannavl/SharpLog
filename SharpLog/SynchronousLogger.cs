using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog
{
    public abstract class SynchronousLogger : Logger
    {
        protected override Task ExecuteAsync(LogLevel level, string text, string callerName)
        {
            return Task.Run(() => Execute(level, text, callerName));
        }

        protected override Task ExecuteAsync(Exception ex, string callerName)
        {
            return Task.Run(() => Execute(ex, callerName));
        }
    }
}

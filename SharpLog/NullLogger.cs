using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLog
{
    using System.Diagnostics;

    public class NullLogger : SynchronousLogger
    {
        protected override void Execute(LogLevel level, string text, string callerName)
        {
            
        }

        protected override void Execute(Exception ex, string callerName)
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}

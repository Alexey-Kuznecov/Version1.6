using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Sinks
{
    public class NullSink : ILogSink
    {
        public void Emit(LogEntry entry)
        {
        }
    }
}

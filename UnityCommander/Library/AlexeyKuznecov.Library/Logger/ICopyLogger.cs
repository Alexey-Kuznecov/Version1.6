
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexeyKuznetsov.Logger
{
    public interface ICopyLogger
    {
        void LogCopyStarted(string source, string destination);
        void LogCopyCompleted(string source, string destination, long size, TimeSpan duration, int fileIndex);
        void LogCopyError(string source, string destination, Exception ex);
    }
}

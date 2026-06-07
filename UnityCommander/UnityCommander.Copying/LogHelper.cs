

using UnityCommander.Core.Logger;

namespace UnityCommander.Copying
{
    public class LogHelper
    {
        public ILogger Log { get; }

        public LogHelper()
        {
            Log = new FileLogger();
        }
    }
}

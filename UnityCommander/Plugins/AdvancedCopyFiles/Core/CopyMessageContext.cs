
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace AdvancedCopyFiles.Core
{
    public class CopyMessageContext
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public CopySessionService? Session { get; set; }

        public CompositeCopySettings? Settings { get; set; }
    }
}

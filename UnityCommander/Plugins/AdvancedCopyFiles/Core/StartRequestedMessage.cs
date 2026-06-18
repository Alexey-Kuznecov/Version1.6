
using AdvancedCopyFiles.Core;

namespace AdvancedCopyFiles.ViewModels
{
    public class StartRequestedMessage
    {
        public CopyMessageContext Context { get; set; }
            = new CopyMessageContext();
    }
}
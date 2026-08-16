
namespace UnityCommander.Abstractions.IO
{
    public class CopyEventBus
    {
        public event EventHandler<CopyInfo>? Progress;
        public event EventHandler<CopyInfo>? Completed;
    }
}

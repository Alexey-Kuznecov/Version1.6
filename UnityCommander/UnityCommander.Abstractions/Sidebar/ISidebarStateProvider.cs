
namespace UnityCommander.Abstractions.Sidebar
{
    public interface ISidebarStateProvider
    {
        byte[] CaptureState();
        
        void RestoreState(byte[] state);
    }
}

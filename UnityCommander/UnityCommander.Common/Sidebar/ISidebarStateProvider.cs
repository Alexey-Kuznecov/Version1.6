
namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarStateProvider
    {
        byte[] CaptureState();
        
        void RestoreState(byte[] state);
    }
}

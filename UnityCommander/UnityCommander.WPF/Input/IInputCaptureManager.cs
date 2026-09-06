
namespace UnityCommander.WPF.Input
{
    public interface IInputCaptureManager
    {
        bool TryHandle(InputEvent e);
        void Push(IInputContext context);
        void Pop();
    }
}

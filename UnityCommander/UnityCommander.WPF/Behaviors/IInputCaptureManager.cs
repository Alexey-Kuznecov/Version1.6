
namespace UnityCommander.WPF.Behaviors
{
    public interface IInputCaptureManager
    {
        bool TryHandle(InputEvent e);
        void Push(IInputContext context);
        void Pop();
    }
}

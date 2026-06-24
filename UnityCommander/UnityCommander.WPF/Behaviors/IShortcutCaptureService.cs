
namespace UnityCommander.WPF.Behaviors
{
    public interface IShortcutCaptureService
    {
        bool IsCapturing { get; }

        void Start(Action<ShortcutInput> handler);
        void Stop();
    }
}

namespace UnityCommander.WPF.Input
{
    public interface IShortcutCaptureService
    {
        bool IsCapturing { get; }

        void Start(Action<ShortcutInput> handler);
        void Stop();
    }
}


namespace UnityCommander.Abstractions.Keyboard
{
    [Flags]
    public enum ShortcutScope
    {
        None = 0,

        Global = 1 << 0,

        MainWindow = 1 << 1,

        FilePanel = 1 << 2,
        Sidebar = 1 << 3,
        Ribbon = 1 << 4,
        Console = 1 << 5,

        Dialog = 1 << 6,

        TextEditor = 1 << 7,
        CodeEditor = 1 << 8,
    }
}

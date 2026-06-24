
namespace UnityCommander.Abstractions.Keyboard
{
    [Flags]
    public enum CommandScope
    {
        None = 0,

        Global = 1 << 0,

        MainWindow = 1 << 1,

        FilePanel = 1 << 2,
        Sidebar = 1 << 3,
        Console = 1 << 4,

        Dialog = 1 << 5,

        TextEditor = 1 << 6,
        CodeEditor = 1 << 7,
    }
}

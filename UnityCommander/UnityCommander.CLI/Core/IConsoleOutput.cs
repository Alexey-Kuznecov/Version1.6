
namespace UnityCommander.CLI.Core
{
    public interface IConsoleOutput
    {
        void Write(string message);
        void WriteLine(string message);

        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add => Console.CancelKeyPress += value;
            remove => Console.CancelKeyPress -= value;
        }

        public void WriteError(string message);
        public void WriteWarning(string message);
        public void WriteSuccess(string message);
        public void Clear();
        public int CursorTop => Console.CursorTop;
        public int CursorLeft => Console.CursorLeft;
        public int WindowWidth => Console.WindowWidth;
        public void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left, top);
        }

        public void SetCursorVisible(bool isVisible)
        {
            Console.CursorVisible = isVisible;
        }
    }
}

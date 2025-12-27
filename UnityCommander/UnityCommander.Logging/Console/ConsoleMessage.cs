
namespace UnityCommander.Logging.Abstractions.Console
{
    public class ConsoleMessage
    {
        public string Text { get; }
        public ConsoleMessageKind Kind { get; }
        public string? Source { get; }

        public ConsoleMessage(
            string text,
            ConsoleMessageKind kind = ConsoleMessageKind.Output,
            string? source = null)
        {
            Text = text;
            Kind = kind;
            Source = source;
        }
    }
}

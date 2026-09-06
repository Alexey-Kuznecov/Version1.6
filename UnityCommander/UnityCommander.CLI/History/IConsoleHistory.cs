
namespace UnityCommander.CLI.History
{
    public interface IConsoleHistory
    {
        IReadOnlyList<string> Items { get; }

        void Add(string command);

        string? Previous();
        string? Next();

        void Reset();
    }
}

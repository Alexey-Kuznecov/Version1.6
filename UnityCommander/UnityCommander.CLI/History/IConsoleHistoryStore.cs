
namespace UnityCommander.CLI.History
{
    public interface IConsoleHistoryStore
    {
        IReadOnlyList<string> Load();

        void Save(IReadOnlyList<string> commands);
    }
}

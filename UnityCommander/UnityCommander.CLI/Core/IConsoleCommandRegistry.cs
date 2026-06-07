
namespace UnityCommander.CLI.Core
{
    public interface IConsoleCommandRegistry
    {
        public IConsoleCommand? Find(string name);
        public void Register(IConsoleCommand command);
        public IReadOnlyCollection<IConsoleCommand> GetAllCommands();
    }
}

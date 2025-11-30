
namespace UnityCommander.CLI.Core
{
    public interface IConsoleCommandDispatcher
    {   
        void Register(IConsoleCommand command);
        bool ExecuteCommand(string commandLine);
        IEnumerable<IConsoleCommand> GetAvailableCommands();
    }
}

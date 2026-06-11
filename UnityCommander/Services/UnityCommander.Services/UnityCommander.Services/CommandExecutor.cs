
using CommandSystem.Abstractions;
using System.Threading.Tasks;

namespace UnityCommander.Services
{
    public class CommandExecutionService
    {
        private readonly IGuiCommandExecutor _executor;

        public CommandExecutionService(
            IGuiCommandExecutor executor)
        {
            _executor = executor;
        }

        public Task ExecuteAsync(string commandName, CommandContext ctx = default)
        {
            return _executor.ExecuteAsync(commandName, ctx);
        }

        internal bool CanExecute(string id)
        {
            return true;
        }
    }
}

using UnityCommander.CLI.Core;

namespace UnityCommander.CLI.Integration
{
    namespace UnityCommander.CLI.Integration
    {
        public interface IConsoleCommandInvoker
        {
            Task InvokeAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default);
        }
    }
}

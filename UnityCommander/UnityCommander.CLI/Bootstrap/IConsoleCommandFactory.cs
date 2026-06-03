
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Bootstrap
{
    public interface IConsoleCommandFactory
    {
        IConsoleCommandBase Create(Type commandType);
    }
}

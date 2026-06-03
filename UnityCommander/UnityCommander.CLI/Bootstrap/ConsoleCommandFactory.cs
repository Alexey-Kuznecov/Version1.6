
using Prism.Ioc;
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Bootstrap
{
    public sealed class ConsoleCommandFactory
        : IConsoleCommandFactory
    {
        private readonly IContainerProvider _container;

        public ConsoleCommandFactory(
            IContainerProvider container)
        {
            _container = container;
        }

        public IConsoleCommandBase Create(Type commandType)
        {
            return (IConsoleCommandBase)
                _container.Resolve(commandType);
        }
    }
}

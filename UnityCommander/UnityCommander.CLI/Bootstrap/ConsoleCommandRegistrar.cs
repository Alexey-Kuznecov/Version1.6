
using Prism.Ioc;
using System.Reflection;
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Bootstrap
{
    public static class ConsoleCommandRegistrar
    {
        public static void RegisterCommands(
            IContainerRegistry registry,
            Assembly assembly)
        {
            var commandTypes =
                ConsoleCommandDiscovery.Discover(assembly);

            foreach (var type in commandTypes)
            {
                registry.Register(
                    typeof(IConsoleCommandBase),
                    type);
            }
        }
    }
}


using System.Reflection;
using UnityCommander.CLI.Integration;
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Bootstrap
{
    public static class ConsoleCommandDiscovery
    {
        public static IEnumerable<Type> Discover(
       Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(IConsoleCommandBase)
                    .IsAssignableFrom(t))
                .Where(t =>
                    t.GetCustomAttribute<ConsoleCommandAttribute>()
                    != null);
        }
    }
}

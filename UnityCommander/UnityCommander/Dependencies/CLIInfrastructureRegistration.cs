
using Prism.Ioc;
using System.IO;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.CLI.History;
using UnityCommander.Logging.Configuration;

namespace UnityCommander.Dependencies
{
    public static class CLIInfrastructureRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<ICliInputAnalyzer, CliInputAnalyzer>();
            registry.RegisterSingleton<ICliParseStateBuilder, CliParseStateBuilder>();

            registry.RegisterSingleton<IConsoleHistory, ConsoleHistory>();
            registry.RegisterSingleton<IConsoleHistory, ConsoleHistory>();

            registry.RegisterSingleton<IConsoleHistoryStore>(sp =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "config", "console-history.json"); ;
                return new JsonConsoleHistoryStore(path);
            });
        }
    }
}

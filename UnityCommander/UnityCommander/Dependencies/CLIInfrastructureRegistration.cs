
using Prism.Ioc;
using UnityCommander.Autocomplete.Infrastructure.Analyze;
using UnityCommander.CLI.History;
using UnityCommander.Common;

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
                var paths = sp.Resolve<UnityCommanderPath>();

                return new JsonConsoleHistoryStore(
                    paths.Config("console-history.json"));
            });
        }
    }
}


using Prism.Ioc;
using UnityCommander.Autocomplete.Infrastructure.Analyze;

namespace UnityCommander.Dependencies
{
    public static class CLIInfrastructureRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<ICliInputAnalyzer, CliInputAnalyzer>();
            registry.RegisterSingleton<ICliParseStateBuilder, CliParseStateBuilder>();
        }
    }
}

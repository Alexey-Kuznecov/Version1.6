
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Testing.Fake;

namespace UnityCommander.Testing.Infrastructure
{
    public static class TestCliEnvironment
    {
        public static TestServiceCatalog CreateGit()
        {
            var catalog = new TestServiceCatalog();

            // Команды
            var commands = new[]
            {
                TestCliInputAnalyzerFactory.CreateGitCommand()
            };

            catalog.RegisterInstance<ICommandCatalog>(
                new CommandCatalog(commands));

            // Анализатор
            catalog.RegisterSingleton<ICliInputAnalyzer>(c =>
                    new DefaultCliInputAnalyzer(
                    c.Resolve<ICommandCatalog>()));
             return catalog;
        }
    }
}

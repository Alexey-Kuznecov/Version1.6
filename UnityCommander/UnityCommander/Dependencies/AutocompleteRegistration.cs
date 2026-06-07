
using Prism.Ioc;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Completion.Providers;
using UnityCommander.Autocomplete.Definitions;
using UnityCommander.Autocomplete.Tokenization;

namespace UnityCommander.Dependencies
{
    public static class AutocompleteRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<ITokenRegistry, TokenRegistry>();
            registry.RegisterSingleton<ICompletionProvider, CommandCompletionProvider>();
            registry.RegisterSingleton<ICompletionProvider, VariantCompletionProvider>();
            registry.RegisterSingleton<ICompletionProvider, ArgumentCompletionProvider>();
            registry.RegisterSingleton<ICompletionProvider, FlagCompletionProvider>();
            registry.RegisterSingleton<IInputTokenizer, SimpleInputTokenizer>();
            registry.RegisterSingleton<ICompletionEngine, CompletionEngine>();

            registry.RegisterInstance<ICommandDescriptor>(new GitCommandDefinition());
            registry.RegisterInstance<ICommandDescriptor>(new PluginCommandDefinition());
        }
    }
}

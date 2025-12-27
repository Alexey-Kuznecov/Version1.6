using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Context.Resolution;
using UnityCommander.Autocomplete.Tokenization;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;
using UnityCommander.Core;
using UnityCommander.Modules.BottomPanel.Completions;
using UnityCommander.Modules.BottomPanel.Views;

namespace UnityCommander.Modules.BottomPanel
{
    public class BottomPanelModule : IModule
    {
        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomPanelModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public BottomPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(BottomPanelView));
            
            regionManager.RequestNavigate(RegionNames.ConsoleTabRegion, nameof(ConsoleView));
            regionManager.RequestNavigate(RegionNames.LogTabRegion, nameof(LogView));
            regionManager.RequestNavigate(RegionNames.PreviewRegion, nameof(PreviewView));
            
            // Internal Console
            //regionManager.RequestNavigate("BottomPanelRegion", "InternalConsole");
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BottomPanelView>();
            containerRegistry.RegisterForNavigation<ConsoleView>();
            containerRegistry.RegisterForNavigation<LogView>();
            containerRegistry.RegisterForNavigation<PreviewView>();

            // Internal Console
            containerRegistry.RegisterSingleton<IConsoleInput, InternalConsoleInput>();
            containerRegistry.RegisterSingleton<IConsoleOutput, InternalConsoleOutput>();

            containerRegistry.RegisterSingleton<ConsoleCommandDispatcher>();
            containerRegistry.RegisterSingleton<ConsoleCommandFactory>();
            containerRegistry.RegisterSingleton<IConsoleCommandRegistry, ConsoleCommandRegistry>();
            containerRegistry.RegisterSingleton<IConsoleCommandInvoker, ConsoleCommandInvoker>();
           
            containerRegistry.RegisterSingleton<ITokenRegistry, TokenRegistry>();
            containerRegistry.RegisterSingleton<ICompletionProvider, ArgumentCompletionProvider>();
            containerRegistry.RegisterSingleton<ICompletionProvider, CommandCompletionProvider>();
            containerRegistry.RegisterSingleton<IInputContextResolver, InputContextResolver>();
            containerRegistry.RegisterSingleton<IInputTokenizer, SimpleInputTokenizer>();
            containerRegistry.RegisterSingleton<ICompletionEngine, CompletionEngine>();

            //containerRegistry.RegisterForNavigation<ConsoleView, ConsoleViewModel>("InternalConsole");
        }
    }
}
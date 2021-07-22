
namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Shapes;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Plugins;

    /// <summary>
    /// The plugin panel view model.
    /// </summary>
    public class PluginPanelViewModel : BindableBase
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The plugin loaders.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaders;

        /// <summary>
        /// The plugin descriptors.
        /// </summary>
        private IEnumerable<IPluginDescriptor> pluginDescriptors;

        /// <summary>
        /// The selected descriptor.
        /// </summary>
        private IPluginDescriptor selectedDescriptor;
        
        /// <summary>
        /// The icon.
        /// </summary>
        private Path icon;

        /// <summary>
        /// The option render.
        /// </summary>
        private IEnumerable<IOption> optionRender;

        /// <summary>
        /// The show dialog command.
        /// </summary>
        private DelegateCommand<object> showDialogCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginPanelViewModel"/> class.
        /// </summary>
        public PluginPanelViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginPanelViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog service.
        /// </param>
        /// <param name="iconProvider">
        /// The icon provider.
        /// </param>
        /// <param name="pluginLoaders">
        /// The plugin loaders.
        /// </param>
        public PluginPanelViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            IPluginLoaderService pluginLoaders)
        {
            this.PluginDescriptors = pluginLoaders.GetPluginDescriptors();
            this.pluginLoaders = pluginLoaders;
            this.dialogService = dialogService;
            this.Icon = iconProvider.GetIcon("Settings").Path;
            this.OptionRender ??= this.pluginLoaders.GetPluginContext().GetOption(null);
        }

        /// <summary>
        /// Gets or sets the selected descriptor.
        /// </summary>
        public IPluginDescriptor SelectedDescriptor
        {
            get => this.selectedDescriptor;
            set
            {
                this.SetProperty(ref this.selectedDescriptor, value);
                this.OptionRender = this.pluginLoaders.GetPluginContext().GetOption(value);
            } 
        }

        /// <summary>
        /// Gets or sets details for plugins provided by the plugin developer.
        /// </summary>
        public IEnumerable<IPluginDescriptor> PluginDescriptors
        {
            get => this.pluginDescriptors;
            set => this.SetProperty(ref this.pluginDescriptors, value);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Path Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        /// <summary>
        /// Gets or sets the option render.
        /// </summary>
        public IEnumerable<IOption> OptionRender
        {
            get => this.optionRender;
            set => this.SetProperty(ref this.optionRender, value);
        }
    }
}

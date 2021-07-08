
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Shapes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using UnityCommander.Core.Mvvm;
using UnityCommander.Integration.Contracts;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public class PluginPanelViewModel : BindableBase
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        private IEnumerable<IPluginDescriptor> pluginDescriptors;

        private Path icon;
        
        private readonly Assembly assembly;

        private DelegateCommand<object> showDialogCommand;

        public PluginPanelViewModel()
        {
        }

        public PluginPanelViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            IEnumerable<IPluginDescriptor> descriptors)
        {
            //var enumerable = descriptors as IPluginDescriptor[] ?? descriptors.ToArray();
            //this.PluginDescriptors = enumerable;
            //this.dialogService = dialogService;
            //this.Icon = iconProvider.GetIcon("Settings").Path;
        }

        /// <summary>
        /// Gets or sets details for plugins provided by the plugin developer.
        /// </summary>
        public IEnumerable<IPluginDescriptor> PluginDescriptors
        {
            get => pluginDescriptors;
            set => SetProperty(ref pluginDescriptors, value);
        }

        public Path Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }

        /// <summary>
        /// The show dialog command.
        /// </summary>
        public DelegateCommand<object> ShowDialogCommand =>
            this.showDialogCommand ??= new DelegateCommand<object>(this.ExecuteShowDialogCommand);

        private void ExecuteShowDialogCommand(object selected)
        {
            this.dialogService.ShowDialog("DialogPluginConfig", new OverrideDialogParameters(selected), r => { });
        }
    }
}

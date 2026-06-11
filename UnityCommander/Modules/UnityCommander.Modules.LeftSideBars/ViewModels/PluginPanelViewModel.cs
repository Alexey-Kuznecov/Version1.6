
namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using Prism.Commands;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System.Collections.Generic;
    using UnityCommander.Common.Plugins;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Plugins;

    public class PluginPanelViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        private DelegateCommand openSettingDialogCommand;

        private IEnumerable<PluginDescriptor> pluginDescriptors;

        private PluginDescriptor selectedDescriptor;

        public PluginPanelViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            IPluginCatalog catalog)
        {
            var descriptors = new List<PluginDescriptor>();
            var icon = iconProvider.GetIcon("Settings");


            foreach (var pluginInfo in catalog.GetAll())
            {
                descriptors.Add(new PluginDescriptor()
                {
                    Author = pluginInfo.Author,
                    Icon = icon,
                    Description = pluginInfo.GetDescription("ru"),
                    Id = pluginInfo.DeveloperID,
                    DisplayName = pluginInfo.Name,
                    Version = pluginInfo.Version
                });
            }

            PluginDescriptors = descriptors;
            _dialogService = dialogService;
        }

        public PluginDescriptor SelectedDescriptor
        {
            get => this.selectedDescriptor;
            set
            {
                this.SetProperty(ref this.selectedDescriptor, value);
            }
        }

        public IEnumerable<PluginDescriptor> PluginDescriptors
        {
            get => this.pluginDescriptors;
            set => this.SetProperty(ref this.pluginDescriptors, value);
        }

        public DelegateCommand OpenSettingDialogCommand =>
          openSettingDialogCommand ??=
              new DelegateCommand(OpenDialogCommand);

        private void OpenDialogCommand()
        {
            _dialogService.ShowDialog("AppConfigDialog");
        }
    }
}

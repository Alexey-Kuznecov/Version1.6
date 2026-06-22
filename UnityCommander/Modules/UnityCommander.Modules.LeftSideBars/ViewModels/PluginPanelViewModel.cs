
namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using Prism.Commands;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System.Collections.Generic;
    using UnityCommander.Abstractions.Resources;
    using UnityCommander.Common.Plugins;
    using UnityCommander.Logging.Infrastructure;
    using UnityCommander.Services.Interfaces.Plugins;

    public class PluginPanelViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        private DelegateCommand openSettingDialogCommand;

        private IEnumerable<PluginDescriptor> pluginDescriptors;

        private PluginDescriptor selectedDescriptor;

        public PluginPanelViewModel(
            IDialogService dialogService,
            CompositeIconResolver iconProvider,
            IPluginCatalog catalog, 
            LoggerCreator loggerCreator)
        {
            var logger = loggerCreator.For<PluginPanelViewModel>();

            var descriptors = new List<PluginDescriptor>();

            //if (!iconProvider.TryResolve("Coin", out var icon))
            //    logger.Warning("Иконка с именем Settings не найдена!");

            foreach (var pluginInfo in catalog.GetAll())
            {
                descriptors.Add(new PluginDescriptor()
                {
                    Author = pluginInfo.Author,
                    IconKey = "Cog",
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

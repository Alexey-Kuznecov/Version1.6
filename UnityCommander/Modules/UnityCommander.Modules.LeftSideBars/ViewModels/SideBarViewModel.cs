
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SidebarViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the SidebarViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Prism.Commands;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using CommandSystem.Abstractions;
    using Prism.Events;
    using Prism.Services.Dialogs;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Controls;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Core;
    using UnityCommander.Integration.Plugins;
    using UnityCommander.Modules.LeftSideBars.Content;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel
    {
        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator mainViewModelExchange;
        private readonly IGuiCommandExecutor _guiCommandExecutor;

        /// <summary>
        /// The pack icon.
        /// </summary>
        private readonly ObservableCollection<IIcon> packIcon;

        /// <summary>
        /// Служба получения диалоговых окон для использования их в VM.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The content control register.
        /// </summary>
        private readonly Dictionary<string, UserControl> contentControlRegister = new Dictionary<string, UserControl>
        {
            { "TableColumn",  new ColumnsOptionControl() },
            { "FileTree", new FolderTreeOverviewControl() },
            { "Comment", new CommentControl() },
            { "Tag", new TagControlPanel() },
            { "Plugin", new PluginControlPanel() },
        };

        /// <summary>
        /// The data context register.
        /// </summary>
        private readonly Dictionary<string, object> dataContextRegister = new Dictionary<string, object>()
        {
            { "TableColumn", null },
            { "FileTree", null },
            { "Comment", null },
            { "Tag", null }
        };

        /// <summary>
        /// The current element of the sidebar.
        /// </summary>
        private SidebarItem currentSidebarItem;

        /// <summary>
        /// The current index of the sidebar element.
        /// </summary>
        private byte currentSideBarItemIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="dialogService">
        /// The service to show window dialogs from the view model.
        /// </param>
        /// <param name="mainViewModelExchange">
        /// Parameter to link to the main view model. 
        /// </param>
        /// <param name="iconProvider">
        /// The service provides icons for the view model.
        /// </param>
        /// <param name="pluginLoader">
        /// The service to load all detected plugin interfaces.
        /// </param>
        public SidebarViewModel(
            IDialogService dialogService,
            IEventAggregator mainViewModelExchange,
            IIconProviderService iconProvider,
            IPluginLoaderService pluginLoader,
            IGlobalCommandService globalCommandService,
            IGuiCommandExecutor guiCommandExecutor)
        {
            this.dialogService = dialogService;
            this.dataContextRegister.Add("Plugin", new PluginPanelViewModel(dialogService, iconProvider, pluginLoader, globalCommandService));
            this.mainViewModelExchange = mainViewModelExchange;
            _guiCommandExecutor = guiCommandExecutor;
            this.packIcon = iconProvider.GetIcons();
            this.CreateSidebarElement();
        }

        /// <summary>
        /// Gets or sets the sidebar content.
        /// </summary>
        public SidebarItem CurrentSidebarItem
        {
            get => this.currentSidebarItem;
            set
            {
                var curr = _guiCommandExecutor.Execute(CommandNames.Panel.GetCurrentPath).Result;
                this.currentSidebarItem = value;
                this.mainViewModelExchange.GetEvent<MessageSendEvent>().Publish(value);
            }
        }

        /// <summary>
        /// Gets or sets the sidebar element.
        /// </summary>
        public ObservableCollection<SidebarItem> SidebarItem { get; set; } = new ObservableCollection<SidebarItem>();

        private DelegateCommand openSettingDialogCommand;
        public DelegateCommand OpenSettingDialogCommand => openSettingDialogCommand ??= new DelegateCommand(OpenDialogCommand);

        void OpenDialogCommand()
        {
            this.dialogService.ShowDialog("AppConfigDialog");
        }

        /// <summary>
        /// Gets or sets the index of the sidebar item is selected.
        /// The current index of the sidebar element that will be passed to the main view model
        /// to indicate which content should be displayed.
        /// </summary>
        public byte CurrentSideBarItemIndex
        {
            get => this.currentSideBarItemIndex;
            set
            {
                this.currentSideBarItemIndex = value;
                this.mainViewModelExchange.GetEvent<MessageSendEvent>().Publish(this.CurrentSideBarItemIndex);
            }
        }

        public PluginPanelViewModel PluginPanelViewModel1
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// Create sidebar element on based .
        /// </summary>
        private void CreateSidebarElement()
        {
            foreach (var content in this.contentControlRegister)
            {
                var icon = this.packIcon.Single(i => ((Icon)i).Category == content.Key);
                var userControl = content.Value;
                userControl.DataContext = this.dataContextRegister[content.Key];

                var sidebarItem = new SidebarItem
                {
                    Content = userControl,
                    Icon = icon
                };

                this.SidebarItem.Add(sidebarItem);
            }
        }
    }
}

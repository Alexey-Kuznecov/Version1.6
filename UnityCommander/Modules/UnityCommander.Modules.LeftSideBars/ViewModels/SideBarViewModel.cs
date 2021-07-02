
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SidebarViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the SidebarViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Controls;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using UnityCommander.Common.Models;
    using UnityCommander.Core;
    using UnityCommander.Integration.Models;
    using UnityCommander.Modules.LeftSideBars.Content;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;
    using UnityCommander.Services.Interfaces;


    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel : BindableBase
    {
        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaderService;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        private Dictionary<string, UserControl> ContentControlRegister = new Dictionary<string, UserControl>()
        {
            { "TableColumn",  new ColumnsOptionControl() },
            { "FileTree", new FolderTreeOverviewControl() },
            { "Comment", new CommentControl() },
            { "Tag", new TagControlPanel() },
            { "Plugin", new PluginControlPanel() },
        };
        
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> DataContextRegister = new Dictionary<string, object>()
        {
            { "TableColumn", null },
            { "FileTree", null },
            { "Comment", null },
            { "Tag", null }
        };

        /// <summary>
        /// The message.
        /// </summary>
        private SidebarItem selectItem;

        /// <summary>
        /// The message.
        /// </summary>
        private byte selectIndex;

        /// <summary>
        /// The show dialog command.
        /// </summary>
        private DelegateCommand showDialogCommand;

        /// <summary>
        /// The pack icon.
        /// </summary>
        private ObservableCollection<IconModel> packIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog service.
        /// </param>
        /// <param name="viewModelMessage">
        /// Communication parameter of the view models. 
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        /// <param name="pluginLoader">
        /// Service for loading all detected plugin interfaces.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public SidebarViewModel(
            IDialogService dialogService,
            IEventAggregator viewModelMessage,
            IIconProviderService iconProvider,
            IPluginLoaderService pluginLoader)
        {
            this.pluginLoaderService = pluginLoader;
            //bool isLoaded = pluginLoaderService.UnloadPlugins();
            //Trace.WriteLine(isLoaded ? "Plugin has been unload" : "Plugin has not been unloaded", "Plugin");

            this.viewModelMessage = viewModelMessage;
            this.packIcon = iconProvider.GetIcons();
            DataContextRegister.Add("Plugin", new PluginPanelViewModel(dialogService, iconProvider, pluginLoader.GetPluginDescriptors()));
            SidebarItems = new ObservableCollection<SidebarItem>();
            this.SidebarContentRender();
        }

        /// <summary>
        /// Gets or sets the sidebar items.
        /// </summary>
        public ObservableCollection<SidebarItem> SidebarItems { get; set; }

        /// <summary>
        /// Gets or sets the sidebar content.
        /// </summary>
        public SidebarItem SelectItem
        {
            get => this.selectItem;
            set
            {
                this.selectItem = value;
                this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the sidebar item is selected.
        /// </summary>
        public byte SelectIndex
        {
            get => this.selectIndex;
            set
            {
                this.selectIndex = value;
                this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.SelectIndex);
            }
        }

        /// <summary>
        /// The sidebar content render.
        /// </summary>
        private void SidebarContentRender()
        {
            foreach (var content in ContentControlRegister)
            {
                var icon = this.packIcon.Single(i => i.Category == content.Key);
                var userControl = content.Value;
                userControl.DataContext = this.DataContextRegister[content.Key];

                var sbItem = new SidebarItem
                {
                    Content = userControl,
                    Icon = icon
                };

                SidebarItems.Add(sbItem);
            }
        }
    }
}

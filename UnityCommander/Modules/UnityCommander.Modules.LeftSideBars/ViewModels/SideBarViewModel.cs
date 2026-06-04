
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
    using MaterialDesignThemes.Wpf;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using UnityCommander.Common.Models;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Common.State;
    using UnityCommander.Integration.Plugins;
    using UnityCommander.Modules.LeftSideBars.Content;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Bootstrap;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel : BindableBase
    {
        private readonly IDialogService dialogService;

        private readonly ObservableCollection<IIcon> packIcon;

        private readonly Dictionary<string, UserControl> contentControlRegister =
            new()
            {
                ["TableColumn"] = new ColumnsOptionControl(),
                ["FileTree"] = new FolderTreeOverviewControl(),
                ["Comment"] = new CommentControl(),
                ["Tag"] = new TagControlPanel(),
                ["Plugin"] = new PluginControlPanel()
            };

        private readonly Dictionary<string, object> dataContextRegister =
            new();

        private DelegateCommand hideSidebarCommand;
        private DelegateCommand openSettingDialogCommand;

        private Path iconHideSidebar;
        private UserControl sidebarContent;
        private int sidebarContentWidth;
        private SidebarItem currentSidebarItem;

        public SidebarViewModel(
            IDialogService dialogService,
            IIconProviderService iconProvider,
            IPluginProvider pluginLoader,
            IMultiCommandService command, 
            ISessionService sessionService)
        {
            this.dialogService = dialogService;

            var session  = sessionService.Load();


            packIcon = iconProvider.GetIcons();

            IconHideSidebar =
                iconProvider.GetIcon(PackIconKind.ArrowBack).GetIconPath();

            dataContextRegister["TableColumn"] = null;
            dataContextRegister["FileTree"] = null;
            dataContextRegister["Comment"] = null;
            dataContextRegister["Tag"] = null;
            dataContextRegister["Plugin"] =
                new PluginPanelViewModel(
                    dialogService,
                    iconProvider,
                    pluginLoader);

            CreateSidebarElement();
        }

        public ObservableCollection<SidebarItem> SidebarItem { get; } = new();

        public Path IconHideSidebar
        {
            get => iconHideSidebar;
            set => SetProperty(ref iconHideSidebar, value);
        }

        public UserControl SidebarContent
        {
            get => sidebarContent;
            set => SetProperty(ref sidebarContent, value);
        }

        public int SidebarContentWidth
        {
            get => sidebarContentWidth;
            set => SetProperty(ref sidebarContentWidth, value);
        }

        public SidebarItem CurrentSidebarItem
        {
            get => currentSidebarItem;
            set
            {
                if (!SetProperty(ref currentSidebarItem, value))
                    return;

                SidebarContent = value?.Content;
                SidebarContentWidth = 250;
            }
        }

        public DelegateCommand HideSidebarCommand =>
            hideSidebarCommand ??= new DelegateCommand(
                () => {
                    SidebarContentWidth = 0;
                    //CurrentSidebarItem = null;
                    //SidebarContent = null;
                });

        public DelegateCommand OpenSettingDialogCommand =>
            openSettingDialogCommand ??=
                new DelegateCommand(OpenDialogCommand);

        internal void Capture(AppSessionState state)
        {
            //state.Sidebar.IsOpen = IsOpen;
        }

        internal void Restore(AppSessionState state)
        {
            //throw new NotImplementedException();
        }

        private void OpenDialogCommand()
        {
            dialogService.ShowDialog("AppConfigDialog");
        }

        private void CreateSidebarElement()
        {
            foreach (var (key, control) in contentControlRegister)
            {
                control.DataContext = dataContextRegister[key];

                SidebarItem.Add(
                    new SidebarItem
                    {
                        Content = control,
                        Icon = packIcon.Single(
                            i => ((Common.Models.Icons.Icon)i).Category == key)
                    });
            }
        }
    }
}


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
    using MaterialDesignThemes.Wpf;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using UnityCommander.Common.Models;
    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Common.State;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Bootstrap;
    using UnityCommander.Services.Interfaces.Sidebar;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        private readonly SidebarService _sidebarService;

        private readonly ObservableCollection<IIcon> packIcon;

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
            ISessionService sessionService,
            SidebarService sidebarService)
        {
            _dialogService = dialogService;

            _sidebarService = sidebarService;

            var session = sessionService.Load();

            packIcon = iconProvider.GetIcons();

            IconHideSidebar =
                iconProvider.GetIcon(PackIconKind.ArrowBack).GetIconPath();
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

        private bool isSidebarOpen;

        public bool IsSidebarOpen
        {
            get => isSidebarOpen;
            set
            {
                if (!SetProperty(ref isSidebarOpen, value))
                    return;

                if (value)
                {
                    SidebarContentWidth = 250;
                    SidebarContent = CurrentSidebarItem?.Content;
                }
                else
                {
                    SidebarContentWidth = 0;
                    SidebarContent = null;
                    CurrentSidebarItem = null;
                }
            }
        }

        public SidebarItem CurrentSidebarItem
        {
            get => currentSidebarItem;
            set
            {
                if (ReferenceEquals(currentSidebarItem, value))
                {
                    IsSidebarOpen = false;
                    return;
                }

                if (!SetProperty(ref currentSidebarItem, value))
                    return;

                SidebarContent = value?.Content;
                IsSidebarOpen = value != null;
            }
        }

        public DelegateCommand HideSidebarCommand =>
            hideSidebarCommand ??=
                new DelegateCommand(() =>
                    IsSidebarOpen = false);

        public DelegateCommand OpenSettingDialogCommand =>
            openSettingDialogCommand ??=
                new DelegateCommand(OpenDialogCommand);

        private void OpenDialogCommand()
        {
            _dialogService.ShowDialog("AppConfigDialog");
        }

        internal void Capture(AppSessionState state)
        {
            state.Sidebar.IsOpen = IsSidebarOpen;
            state.Sidebar.ActiveSectionId = CurrentSidebarItem.Id;
        }

        internal void Restore(AppSessionState state)
        {
            IsSidebarOpen = state.Sidebar.IsOpen;

            var item = SidebarItem
             .FirstOrDefault(x => x.Id == state.Sidebar.ActiveSectionId)
                ?? SidebarItem.FirstOrDefault();

            SidebarContent = item?.Content;
            CurrentSidebarItem = item;
        }

        internal void Initialize()
        {
            foreach (var item in _sidebarService.GetAll().ToList())
            {
                item.View.DataContext = item?.ViewModel;

                SidebarItem.Add(
                    new SidebarItem
                    {
                        Id = item.Id,
                        Content = item.View,
                        Icon = packIcon.Single(
                            i => ((Common.Models.Icons.Icon)i).Category == item.IconKey)
                    });
            }
        }
    }
}

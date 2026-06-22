
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
    using CommandSystem.Infrastructure.Lifecycle;
    using MaterialDesignThemes.Wpf;
    using Prism.Dialogs;
    using Prism.Mvvm;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Helper;
    using UnityCommander.Common.Models;
    using UnityCommander.Common.State;
    using UnityCommander.Common.States;
    using UnityCommander.Core.Commands;
    using UnityCommander.Rendering.Icons;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Bootstrap;
    using UnityCommander.Services.Interfaces.Plugins;
    using UnityCommander.Services.Interfaces.Sidebar;
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
    using static UnityCommander.Common.Commands.CommandNames;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel : BindableBase
    {
        private readonly IViewResolver _viewResolver;

        private readonly IDialogService _dialogService;

        private readonly ISidebarService _sidebarService;

        private readonly IIconRenderService _iconResolver;

        private DelegateCommand hideSidebarCommand;

        private DelegateCommand openSettingDialogCommand;

        private SidebarSessionState _state;

        private Path iconHideSidebar;

        private UserControl sidebarContent;

        private int sidebarContentWidth;

        private SidebarItem currentSidebarItem;

        public SidebarViewModel(
            IDialogService dialogService,
            IIconRenderService iconResolver,
            IPluginInfoProvider pluginLoader,
            IMultiCommandService command,
            ISessionService sessionService,
            IViewResolver viewResolver,
            ISidebarService sidebarService, 
            CommandRegistryService commandRegistry)
        {
            _viewResolver = viewResolver;

            _iconResolver = iconResolver;

            _dialogService = dialogService;

            _sidebarService = sidebarService;

            IconHideSidebar = iconResolver.GetPath(Navigation.Back);

            sidebarService.OnCleanup += SidebarService_PluginUnloaded;

            commandRegistry.Register(CommandFactoryExtensions.Create(
              UI.ToggleSidebar,
              ToggleSidebar
          ));
        }

        private void SidebarService_PluginUnloaded(string pluginId)
        {
            var itemsToRemove = SidebarItems
                .Where(x => x.Owner == pluginId)
                .ToList();

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in itemsToRemove)
                {
                    SidebarItems.Remove(item);
                }
            });
        }

        public ObservableCollection<SidebarItem> SidebarItems { get; } = new();

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

        public bool IsSidebarOpen => _state.IsOpen;

        public SidebarItem CurrentSidebarItem
        {
            get => currentSidebarItem;
            set
            {
                SetProperty(ref currentSidebarItem, value);
                Open(currentSidebarItem);
            }
        }

        public Task ToggleSidebar(CommandContext ctx)
        {
            _state.IsOpen = !_state.IsOpen;
            _state.ActiveSectionId = _state.IsOpen ? currentSidebarItem?.Id : null;

            Apply();

            return Task.CompletedTask;
        }

        public void Open(SidebarItem item)
        {
            _state.IsOpen = true;
            _state.ActiveSectionId = item?.Id;

            Apply();
        }

        public void Close()
        {
            _state.IsOpen = false;
            _state.ActiveSectionId = null;

            Apply();
        }

        internal void Capture(AppSessionState state)
        {
            state.Sidebar.ActiveSectionId = CurrentSidebarItem?.Id;
        }

        internal void Restore(AppSessionState state)
        {
            _state = state.Sidebar;
            Apply();
        }

        internal void Initialize()
        {
            SidebarItems.Clear();

            foreach (var item in _sidebarService.GetAll().ToList())
            {
                var view = (UserControl)_viewResolver.Resolve(item.ViewType);

                view.DataContext = _viewResolver.Resolve(item.ViewModel);

                SidebarItems.Add(
                    new SidebarItem
                    {
                        Id = item.Id,
                        Content = view,
                        Owner = item.OwnerId,
                        IconKey = item.IconKey
                    });
            }
        }

        private void Apply()
        {
            var item = SidebarItems
                .FirstOrDefault(x => x.Id == _state.ActiveSectionId);

            SidebarContent = _state.IsOpen ? item?.Content : null;
            SidebarContentWidth = _state.IsOpen ? 300 : 0;
        }

        public DelegateCommand HideSidebarCommand =>
            hideSidebarCommand ??=
                new DelegateCommand(() =>
                    Close());

        public DelegateCommand OpenSettingDialogCommand =>
            openSettingDialogCommand ??=
                new DelegateCommand(OpenDialogCommand);

        private void OpenDialogCommand()
        {
            _dialogService.ShowDialog("AppConfigDialog");
        }
    }
}

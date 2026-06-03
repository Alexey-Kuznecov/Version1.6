// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="T">
//  Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the MainWindowViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace UnityCommander.ViewModels
{
    using AlexeyKuznecov.Library.Mvvm.Base;
    using MaterialDesignThemes.Wpf;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models;
    using UnityCommander.Controls.Window;
    using UnityCommander.Core;
    using UnityCommander.Core.Behaviors;
    using UnityCommander.Ribbon.Core.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Settings;
    using Path = System.Windows.Shapes.Path;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase, IKeyBinding
    {
        #region DECLARATION FIELDS

        private readonly IRibbonManager _ribbonManager;

        /// <summary>
        /// The index of the sidebar item.
        /// </summary>
        private static byte sidebarItemIndex;

        /// <summary>
        /// The width of the sidebar content.
        /// </summary>
        private int sidebarContentWidth;

        /// <summary>
        /// The sidebar content.
        /// </summary>
        private UserControl sidebarContent;

        #region DEPENDENCY INJECTION PROPERTIES

        /// <summary>
        /// The application commands.
        /// </summary>
        private IMultiCommandService stateCommands;

        /// <summary>
        /// The application commands.
        /// </summary>
        private readonly IGlobalCommandManager globalCommandManager;

        #endregion

        /// <summary>
        /// The import view model of the custom window.
        /// </summary>
        private CustomViewModel importCustomWindow;

        /// <summary>
        /// The icon hide sidebar.
        /// </summary>
        private Path iconHideSidebar;

        /// <summary>
        /// The icon hide sidebar.
        /// </summary>
        private string icon;

        #region MINIMIZE TOOLBAR FIELDS

        /// <summary>
        /// Size of the tab container .
        /// </summary>
        private double tabContainerSize;

        /// <summary>
        /// Size of the ribbon container.
        /// </summary}
        private double ribbonContainerSize;

        /// <summary>
        /// Visibility of the toolbar.
        /// </summary>
        private Visibility toolBarVisibility;

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">
        /// The service to manage a dialog window.
        /// </param>
        /// <param name="exchange">
        /// The service to exchange of information between view models.
        /// </param>
        /// <param name="settingsProviderService">
        /// The service to configure of application.
        /// </param>
        /// <param name="iconProviderService">
        /// The service to provide icons.
        /// </param>
        /// <param name="command">
        /// The service to execute commands from each view model if the command is registered globally.
        /// </param>
        /// <param name="globalCommand">
        /// 
        /// </param>
        public MainWindowViewModel(
            IDialogService dialogService,
            IEventAggregator exchange,
            ISettingsProviderService settingsProviderService,
            IIconProviderService iconProviderService,
            IMultiCommandService command,
            IGlobalCommandService globalCommand, IRibbonManager ribbonManager)
        {
            _ribbonManager = ribbonManager;
            this.StateCommand = command;
            this.globalCommandManager = globalCommand.GetCommandManager();
            
            this.StateCommand
                .SaveCommand
                .RegisterCommand(this.CloseWindowCommand);

            exchange.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);

            var settings = settingsProviderService.GetAppConfig();
            
            this.SidebarContentWidth 
                = settings.SidebarDisplayContent ? 250 : 0;
           
            this.IsRibbonExpanded = false;
            this.IconHideSidebar 
                = iconProviderService.GetIcon(PackIconKind.ArrowBack).GetIconPath();
            
            this.Icon = Directory.GetCurrentDirectory() + "\\icon.ico";
            
            _ribbonManager.TabCollapsed += RibbonManager_TabCollapsed;
            _ribbonManager.TabExpanded += RibbonManager_TabExpanded;
        }

        private void RibbonManager_TabExpanded(object sender, RibbonTabEventArgs e)
        {
            RibbonContainerSize = 180;
        }

        private void RibbonManager_TabCollapsed(object sender, RibbonTabEventArgs e)
        {
            RibbonContainerSize = 38;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the icon hide sidebar.
        /// </summary>
        public Path IconHideSidebar
        {
            get => this.iconHideSidebar;
            set => this.SetProperty(ref this.iconHideSidebar, value);
        }

        /// <summary>
        /// Gets or sets the icon hide sidebar.
        /// </summary>
        public string Icon
        {
            get => this.icon;
            set => this.SetProperty(ref this.icon, value);
        }
        
        /// <summary>
        /// Gets or sets the view model. View model, used to control custom window buttons.
        /// </summary>
        public CustomViewModel ImportCustomWindow
        {
            get => this.importCustomWindow;
            set
            {
                if (value != null)
                {
                    value.CloseCommand = this.StateCommand.SaveCommand;
                    this.SetProperty(ref this.importCustomWindow, value);
                    this.importCustomWindow.CollapseRibbonCommand = new RelayCommand(obj =>
                    {
                        //this.importCustomWindow.GridRowShadow = this.importCustomWindow.GridRowShadow == 0 ? 10 : 0;
                        this.importCustomWindow.CollapseContent = (this.importCustomWindow.CollapseContent as string) == "Max" ? "Mix" : "Max";
                        IsRibbonExpanded = !IsRibbonExpanded;
                    });
                }
            }
        }

        /// <summary> 
        /// Gets or sets the application commands.
        /// </summary>
        public IMultiCommandService StateCommand
        {
            get => this.stateCommands;
            set => this.SetProperty(ref this.stateCommands, value);
        }

        #endregion

        /// <summary>
        /// The command to close window.
        /// </summary>
        public DelegateCommand<Window> CloseWindowCommand => new DelegateCommand<Window>(
            window =>
            {
                Application.Current.Shutdown();
            });

        #region MINIMIZE TOOLBAR PROPERTIES

        /// <summary>
        /// Gets or sets the size of element that contains toolbar tabs.
        /// </summary>
        public double TabContainerSize
        {
            get => this.tabContainerSize;
            set => this.SetProperty(ref this.tabContainerSize, value);
        }

        /// <summary>
        /// Gets or sets the size of element that contains the tool ribbon.
        /// </summary>
        public double RibbonContainerSize
        {
            get => this.ribbonContainerSize;
            set => this.SetProperty(ref this.ribbonContainerSize, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates the visibility of the toolbar.
        /// </summary>
        public Visibility ToolBarVisibility
        {
            get => this.toolBarVisibility;
            set => this.SetProperty(ref this.toolBarVisibility, value);
        }

        private bool _isRibbonExpanded;

        public bool IsRibbonExpanded
        {
            get => _isRibbonExpanded;
            set
            {
                if (!SetProperty(ref _isRibbonExpanded, value))
                    return;

                RibbonContainerSize = value ? 180 : 0;
                ToolBarVisibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion

        /// <summary>
        /// The set binding.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public void SetBinding(object dependencyObject, KeyboardManager manager)
        {
            Grid grid = dependencyObject as Grid;

            foreach (var globalCommand in this.globalCommandManager.GetCommands().Where(globalCommand => globalCommand?.ShortcutKey != null))
            {
                grid?.InputBindings.Add(new InputBinding(globalCommand.Command, globalCommand.ShortcutKey));
            }
        }

        #region SIDEBAR MEMBER

        /// <summary>
        /// Gets or sets the content of sidebar.
        /// </summary>
        public UserControl SidebarContent
        {
            get => this.sidebarContent;
            set => this.SetProperty(ref this.sidebarContent, value);
        }

        /// <summary>
        /// Gets or sets the width of sidebar content.
        /// </summary>
        public int SidebarContentWidth
        {
            get => this.sidebarContentWidth;
            set => this.SetProperty(ref this.sidebarContentWidth, value);
        }

        /// <summary>
        /// The command to hide the content of the sidebar.
        /// </summary>
        public DelegateCommand HideSidebarCommand => new DelegateCommand(
            () =>
                {
                    this.SidebarContentWidth = 0;
                });

        /// <summary>
        /// The set sidebar view model.
        /// </summary>
        /// <param name="obj"> The sidebar view. </param>
        private void SetSidebarViewModel(object obj)
        {
            if (obj is SidebarItem sideBarContent)
            {
                this.SidebarContent = sideBarContent.Content;
            }

            if (obj is not byte index) return;
            this.SidebarContentWidth = index == sidebarItemIndex ? 0 : 250;
            sidebarItemIndex = index;
        }

        #endregion
    }
}

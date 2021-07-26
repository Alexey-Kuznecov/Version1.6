
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="T">
//  Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the MainWindowViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.ViewModels
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using MaterialDesignThemes.Wpf;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using UnityCommander.Common.Models;
    using UnityCommander.Core;
    using UnityCommander.Services.Interfaces;

    using WindowCustomizer;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region DECLARATION FIELDS

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
        private IGlobalCommandService stateCommands;

        #endregion
        
        /// <summary>
        /// The import view model of the custom window.
        /// </summary>
        private CustomViewModel importCustomWindow;

        /// <summary>
        /// The icon hide sidebar.
        /// </summary>
        private Path iconHideSidebar;

        #region MINIMIZE TOOLBAR FIELDS

        /// <summary>
        /// Size of the tab container .
        /// </summary>
        private double tabContainerSize;

        /// <summary>
        /// Size of the ribbon container.
        /// </summary>
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
        public MainWindowViewModel(
            IDialogService dialogService,
            IEventAggregator exchange, 
            ISettingsProviderService settingsProviderService,
            IIconProviderService iconProviderService,
            IGlobalCommandService command)
        {
            this.StateCommand = command;
            this.StateCommand.SaveCommand.RegisterCommand(this.CloseWindowCommand);

            exchange.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);
            
            var settings = settingsProviderService.GetAppConfig();
            this.SidebarContentWidth = settings.SidebarDisplayContent ? 250 : 0;
            
            this.RibbonContainerSize = 50;
            this.TabContainerSize = 80;
            
            this.IconHideSidebar = iconProviderService.GetIcon(PackIconKind.ArrowBack).GetIconPath();
        }

        /// <summary>
        /// Gets or sets the icon hide sidebar.
        /// </summary>
        public Path IconHideSidebar
        {
            get => this.iconHideSidebar;
            set => this.SetProperty(ref this.iconHideSidebar, value);
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
                }
            }
        }

        /// <summary>
        /// Gets or sets the application commands.
        /// </summary>
        public IGlobalCommandService StateCommand
        {
            get => this.stateCommands;
            set => this.SetProperty(ref this.stateCommands, value);
        }

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

        /// <summary>
        /// The command to minimize and maximize the toolbar.
        /// </summary>
        public DelegateCommand MinimizeToolbarCommand => new DelegateCommand(() =>
        {
            if (this.RibbonContainerSize > 0)
            {
                this.RibbonContainerSize = 0;
                this.TabContainerSize = 0;
                this.ToolBarVisibility = Visibility.Hidden;
            }
            else
            {
                this.RibbonContainerSize = 50;
                this.TabContainerSize = 80;
                this.ToolBarVisibility = Visibility.Visible;
            }
        });

        #endregion

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

            if (obj is byte index)
            {
                this.SidebarContentWidth = index == sidebarItemIndex ? 0 : 250;
                sidebarItemIndex = index;
            }
        }

        #endregion
    }
}

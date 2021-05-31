
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
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Common.Models;
    using UnityCommander.Core;
    using UnityCommander.Services.Interfaces;
    using WindowCustomizer;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// The index of the sidebar item that used  .
        /// </summary>
        private static byte sidebarItemIndex;

        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;

        /// <summary>
        /// The application commands.
        /// </summary>
        private IGlobalCommandService stateCommands;

        /// <summary>
        /// The application settings.
        /// </summary>
        private ISettings settingsService;

        /// <summary>
        /// The drag increment.
        /// </summary>
        private UserControl sidebarContent;

        /// <summary>
        /// The import custom window.
        /// </summary>
        private CustomViewModel importCustomWindow;

        /// <summary>
        /// The sidebar content width.
        /// </summary>
        private int sidebarContentWidth;

        /// <summary>
        /// The title.
        /// </summary>
        private string title = "Prism Application";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="message">
        /// Gets interface to exchange of information between view models.
        /// </param>
        /// <param name="settings">
        /// Gets interface to configure of application.
        /// </param>
        /// <param name="command">
        /// Gets interface to execute commands each view model at the time.
        /// </param>
        public MainWindowViewModel(IEventAggregator message, ISettingsProviderService settings, IGlobalCommandService command)
        {
            this.viewModelMessage = message;
            this.StateCommand = command;
            this.settingsService = settings.GetAppConfig();
            this.StateCommand.SaveCommand.RegisterCommand(this.CloseWindowCommand);
            this.viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);
            this.SidebarContentWidth = this.settingsService.SidebarDisplayContent ? 250 : 0;
        }

        /// <summary>
        /// Gets or sets the custom window.
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
        /// Gets or sets the sidebar content width.
        /// </summary>
        public int SidebarContentWidth
        {
            get => this.sidebarContentWidth;
            set => this.SetProperty(ref this.sidebarContentWidth, value);
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        /// <summary>
        /// Gets or sets the drag increment.
        /// </summary>
        public UserControl SidebarContent
        {
            get => this.sidebarContent;
            set => this.SetProperty(ref this.sidebarContent, value);
        }

        /// <summary>
        /// The close window command.
        /// </summary>
        public DelegateCommand<Window> CloseWindowCommand => new DelegateCommand<Window>(window =>
        {
            window.Close();
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
    }
}

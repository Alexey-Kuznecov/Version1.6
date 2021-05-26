
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
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Core;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public partial class MainWindowViewModel : BindableBase
    { 
        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;

        /// <summary>
        /// The title.
        /// </summary>
        private string title = "Prism Application";

        /// <summary>
        /// The application commands.
        /// </summary>
        private ICommonStateService stateCommands;

        /// <summary>
        /// The drag increment.
        /// </summary>
        private UserControl sidebarContent;

        /// <summary>
        /// The sidebar content width.
        /// </summary>
        private int sidebarContentWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="message">
        /// The view Model Message.
        /// </param>
        /// <param name="commandService">
        /// The service that respond for composite commands.
        /// </param>
        public MainWindowViewModel(IEventAggregator message, ICommonStateService commandService)
        {
            this.StateCommand = commandService;
            this.StateCommand.SaveCommand.RegisterCommand(this.CloseWindowCommand);
            this.viewModelMessage = message;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);
        }

        /// <summary>
        /// Gets or sets the application commands.
        /// </summary>
        public ICommonStateService StateCommand
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
            this.SidebarContentWidth = 250;
            this.SidebarContent = obj as UserControl;
        }
    }
}


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
    using Prism.Events;
    using Prism.Mvvm;
    using System;
    using System.Windows.Controls;
    using UnityCommander.Core;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase
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
        /// The drag increment.
        /// </summary>
        private UserControl sidebarContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IEventAggregator viewModelMessage)
        {
            this.viewModelMessage = viewModelMessage;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);
        }

        private void SetSidebarViewModel(object obj)
        {
            this.SidebarContent = obj as UserControl;
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
    }
}

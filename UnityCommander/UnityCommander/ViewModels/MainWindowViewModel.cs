
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
    using System.Windows.Controls;

    using Prism.Events;
    using Prism.Mvvm;

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
        /// The sidebar content width.
        /// </summary>
        private int sidebarContentWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="message">
        /// The view Model Message.
        /// </param>
        public MainWindowViewModel(IEventAggregator message)
        {
            this.viewModelMessage = message;
            this.viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetSidebarViewModel);
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

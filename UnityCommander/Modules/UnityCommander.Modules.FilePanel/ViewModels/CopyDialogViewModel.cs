
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyDialogViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//  The class is a view model for dialog window of the copy files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Core;
    using UnityCommander.Modules.FilePanel.Views;

    /// <summary>
    /// The class is a view model and serves to initial files copying settings.
    /// </summary>
    public class CopyDialogViewModel : BindableBase
    {
        #region Declaration Fields

        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;

        /// <summary>
        /// Contains the path to the source panel.
        /// </summary>
        private string source;

        /// <summary>
        /// Contains the path to the target panel.
        /// </summary>
        private string target;

        /// <summary>
        /// Contains a view of the copy dialog box.
        /// </summary>
        private UserControl controlView;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDialogViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyDialogViewModel(IEventAggregator viewModelMessage)
        {
            viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.MessageReceived);
            this.ControlView = new CopyDialogControl();
            this.CopyCommand = new DelegateCommand(this.CopyExecute);
            this.viewModelMessage = viewModelMessage;
        }

        #endregion

        #region Declaration Properties

        /// <summary>
        /// Gets or sets the appearance of the copy dialog box.
        /// </summary>
        public UserControl ControlView
        {
            get => this.controlView;
            set => this.SetProperty(ref this.controlView, value);
        }

        /// <summary>
        /// Gets or sets the source panel.
        /// </summary>
        public string Source
        {
            get => this.source;
            set => this.SetProperty(ref this.source, value);
        }

        /// <summary>
        /// Gets or sets the target panel.
        /// </summary>
        public string Target
        {
            get => this.target;
            set => this.SetProperty(ref this.target, value);
        }

        /// <summary>
        /// Gets or sets a list masks or templates for file extensions
        /// that will be included in copying.
        /// </summary>
        public List<string> IncludeExtension { get; set; }

        /// <summary>
        /// Gets or sets a list masks or templates for file extensions 
        /// that will be excluded of copying. 
        /// </summary>
        public List<string> ExcludeExtension { get; set; }

        /// <summary>
        /// Gets the command to copy files.
        /// </summary>
        public ICommand CopyCommand { get; }

        #endregion

        #region Methods Implementation 

        /// <summary>
        /// Copies files or folders from one panel to another.
        /// </summary>
        public void CopyExecute()
        {
            this.ControlView = new CopyProcessView();
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(new[] { this.Source, this.Target });
        }

        /// <summary>
        /// Gets a message from the sidebar region.
        /// </summary>
        /// <param name="message"> Message of the sidebar. </param>
        private void MessageReceived(object message)
        {
            MessageBox.Show((string)message);
        }

        #endregion
    }
}

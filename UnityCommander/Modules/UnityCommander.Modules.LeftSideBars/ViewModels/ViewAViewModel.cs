
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewAViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the ViewAViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Business;
    using UnityCommander.Core;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class ViewAViewModel : BindableBase
    {        
        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;
        
        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public ViewAViewModel(IEventAggregator viewModelMessage)
        {
            this.viewModelMessage = viewModelMessage;
            this.SendCopyInfoCommand = new DelegateCommand(this.SendMessage);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        /// <summary>
        /// Gets the send copy info command.
        /// </summary>
        public DelegateCommand SendCopyInfoCommand { get; }

        /// <summary>
        /// The send copy info.
        /// </summary>
        private void SendMessage()
        {
            this.viewModelMessage.GetEvent<MessageSendEvent>().Publish("It is works!");
        }
    }
}

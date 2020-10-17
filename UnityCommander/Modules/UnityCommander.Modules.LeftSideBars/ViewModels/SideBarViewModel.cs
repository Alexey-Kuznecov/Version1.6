
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SidebarViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   Defines the SidebarViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    using System.Windows.Controls;

    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Core;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;

    /// <summary>
    /// The view a view model.
    /// </summary>
    public class SidebarViewModel : BindableBase
    {        
        /// <summary>
        /// The view model message.
        /// </summary>
        private readonly IEventAggregator viewModelMessage;
        
        /// <summary>
        /// The message.
        /// </summary>
        private ListViewItem selectItem;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public SidebarViewModel(IEventAggregator viewModelMessage)
        {
            this.viewModelMessage = viewModelMessage;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public ListViewItem SelectItem
        {
            get => this.selectItem;
            set
            {
                this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(new ColumnsOptionControl());
            }
        }
    }
}

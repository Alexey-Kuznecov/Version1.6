
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;

    using Prism.Events;
    using Prism.Mvvm;

    using UnityCommander.Common.Models;
    using UnityCommander.Core;
    using UnityCommander.Integration.Models;
    using UnityCommander.Modules.LeftSideBars.Content;
    using UnityCommander.Modules.LeftSideBars.SidebarContent;
    using UnityCommander.Services.Interfaces;

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
        /// Gets or sets the type.
        /// </summary>
        private Dictionary<string, UserControl> sidebarContentControl = new Dictionary<string, UserControl>()
        {
            { "TableColumn",  new ColumnsOptionControl() },
            { "FileTree", new FolderTreeOverviewControl() },
            { "Comment", new CommentControl() },
            { "Tag", new TagControlPanel() },
            { "Plugin", new PluginControlPanel() },
        };

        /// <summary>
        /// The message.
        /// </summary>
        private SidebarItem selectItem;

        /// <summary>
        /// The message.
        /// </summary>
        private byte selectIndex;

        /// <summary>
        /// The pack icon.
        /// </summary>
        private ObservableCollection<IconModel> packIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="SidebarViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage">
        /// Communication parameter of the view models. 
        /// </param>
        /// <param name="iconProvider">
        /// The icon Provider.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public SidebarViewModel(IEventAggregator viewModelMessage, IIconProviderService iconProvider)
        {
            this.viewModelMessage = viewModelMessage;
            this.packIcon = iconProvider.GetIcons();
            SidebarItems = new ObservableCollection<SidebarItem>();
            this.SidebarContentRender();
        }

        /// <summary>
        /// Gets or sets the sidebar items.
        /// </summary>
        public ObservableCollection<SidebarItem> SidebarItems { get; set; }

        /// <summary>
        /// Gets or sets the sidebar content.
        /// </summary>
        public SidebarItem SelectItem
        {
            get => this.selectItem;
            set
            {
                this.selectItem = value;
                this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the sidebar item is selected.
        /// </summary>
        public byte SelectIndex
        {
            get => this.selectIndex;
            set
            {
                this.selectIndex = value;
                this.viewModelMessage.GetEvent<MessageSendEvent>().Publish(this.SelectIndex);
            }
        }

        /// <summary>
        /// The sidebar content render.
        /// </summary>
        private void SidebarContentRender()
        {
            foreach (var icon in this.packIcon)
            {
                var sbItem = new SidebarItem
                {
                    Content = this.sidebarContentControl[icon.Category],
                    Icon = icon
                };

                SidebarItems.Add(sbItem);
            }
        }
    }
}

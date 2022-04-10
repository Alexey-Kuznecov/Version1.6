
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Ribbon.Control;

    /// <summary>
    /// TODO The ribbon group builder.
    /// </summary>
    public class RibbonGroupBuilder
    {
        /// <summary>
        /// The group.
        /// </summary>
        private RibbonGroup group;

        /// <summary>
        /// The group adorner.
        /// </summary>
        private RibbonGroupAdorner groupAdorner;

        /// <summary>
        /// The add group.
        /// </summary>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonGroup"/>.
        /// </returns>
        public RibbonGroupBuilder AddGroup(string groupName)
        {
            var ab = new RibbonGroupAdorner();
            this.group = new RibbonGroup();
            this.groupAdorner = ab.SetAdorner(groupName, this.group);
            return this;
        }

        /// <summary>
        /// The add button.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonGroupBuilder"/>.
        /// </returns>
        public RibbonGroupBuilder AddButton(object content, IIcon icon, ICommand command)
        {
            RibbonItem item = new RibbonItem(new RibbonButton(content, icon, command));
            this.group.Children.Add(item);
            return this;
        }

        /// <summary>
        /// The add item group.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItemGroup(Action<RibbonItemGroup> item)
        {
            RibbonItemGroup ribbonItemGroup = new RibbonItemGroup();
            ribbonItemGroup.AddItem(item);
            this.group.Children.Add(ribbonItemGroup);
        }

        /// <summary>
        /// The add combo box.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddComboBox(Action<RibbonItemGroup> item)
        {
            RibbonItemGroup ribbonItemGroup = new RibbonItemGroup();
            ribbonItemGroup.AddItem(item);
            this.group.Children.Add(ribbonItemGroup);
        }

        /// <summary>
        /// The get group.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonGroup"/>.
        /// </returns>
        internal RibbonGroup GetGroup() => this.group;

        /// <summary>
        /// The get group.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonGroupAdorner"/>.
        /// </returns>
        internal RibbonGroupAdorner GetAdorner() => this.groupAdorner;
    }
}

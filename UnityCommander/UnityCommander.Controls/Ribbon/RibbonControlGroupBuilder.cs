
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Ribbon.Control;

    /// <summary>
    /// TODO The ribbon group builder.
    /// </summary>
    public class RibbonControlGroupBuilder
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
        public RibbonControlGroupBuilder AddGroup(string groupName)
        {
            var ab = new RibbonGroupAdorner();
            this.group = new RibbonGroup();
            this.groupAdorner = ab.SetAdorner(groupName, this.group);
            return this;
        }

        /// <summary>
        /// The add group.
        /// </summary>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonControlGroupBuilder"/>.
        /// </returns>
        public RibbonControlGroupBuilder AddGroup(string groupName, Action<RibbonControlGroupBuilder> callback)
        {
            var ab = new RibbonGroupAdorner();
            this.group = new RibbonGroup();
            this.groupAdorner = ab.SetAdorner(groupName, this.group);
            callback(this);
            return this;
        }

        /// <summary>
        /// Adds a new tool ribbon button.
        /// </summary>
        /// <param name="name">
        /// The button name.
        /// </param>
        /// <param name="icon">
        /// The button icon.
        /// </param>
        /// <param name="command">
        /// The button command.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonControlGroupBuilder"/>. Returns the control group designer for further tool ribbon construction . 
        /// </returns>
        /// <remarks> Controls will be added in the order in which this function is called. </remarks>
        public RibbonControlGroupBuilder AddButton(string name, IIcon icon, ICommand command)
        {
            RibbonElement element = new RibbonElement(new RibbonButton(name, icon, command));
            this.group.Children.Add(element);
            return this;
        }

        /// <summary>
        /// Adds a list of controls to the tool ribbon.
        /// </summary>
        /// <param name="callback">
        /// Callback function to add a control to the Tool Ribbon.
        /// </param>
        /// <remarks> Each control is added from top to bottom. </remarks>
        public void AddControlList(Action<RibbonControlList> callback)
        {
            using var ribbonControlList = new RibbonControlList();
            ribbonControlList.AddItem(callback);
            this.group.Children.Add(ribbonControlList);
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

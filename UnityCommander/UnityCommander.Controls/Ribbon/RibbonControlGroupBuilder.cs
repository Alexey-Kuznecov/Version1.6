
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Controls.Ribbon.Control;
    using UnityCommander.Controls.Ribbon.Subgroup;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// This class is responsible for building a group of controls for the Tool Ribbon.
    /// </summary>
    public class RibbonControlGroupBuilder
    {
        /// <summary>
        /// An instance of a subgroup object in a group on the tool ribbon.
        /// </summary>
        private BaseSubgroup baseSubgroup;

        /// <summary>
        /// The object instance for a group of controls.
        /// </summary>
        private RibbonGroup group;

        /// <summary>
        /// Wrapper object for a group of controls
        /// </summary>
        private RibbonGroupAdorner groupAdorner;

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
        public RibbonControlGroupBuilder AddButton(string name, IIcon icon, RibbonCommand command)
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
        /// <remarks> Each control will be located one above the other. </remarks>
        public void AddList(Action<RibbonControlListBuilder> callback)
        {
            using var ribbonControlList = new RibbonControlListBuilder();
            ribbonControlList.AddItem(callback);
            this.group.Children.Add(ribbonControlList.ControlsStackGroup);
        }

        /// <summary>
        /// Creates a group of controls for the Tool Ribbon.. 
        /// </summary>
        /// <remarks>
        /// The <see cref="RibbonGroupAdorner.SetAdorner(string, RibbonGroup)"/> method separates group names from controls.
        /// </remarks>
        /// <param name="groupName">
        /// The name for the group of controls.
        /// </param>
        /// <param name="callback">
        /// The callback function passes the control group build object as a parameter.
        /// </param>
        /// <returns>
        /// Returns the <see cref="RibbonControlGroupBuilder"/> object to continue building a group of controls.
        /// </returns>
        internal RibbonControlGroupBuilder AddGroup(string groupName, Action<RibbonControlGroupBuilder> callback)
        {
            var rga = new RibbonGroupAdorner();
            this.group = new RibbonGroup();
            this.groupAdorner = rga.SetAdorner(groupName, this.group);
            callback(this);
            return this;
        }

        /// <summary>
        /// Gets the wrapper object for the group of controls.
        /// </summary>
        /// <remarks>
        /// Note, that this <see cref="RibbonGroupAdorner"/> object, pre-wraps the <see cref="RibbonGroup"/> object.
        /// </remarks>
        [NJsonSchema.Annotations.NotNull]
        internal RibbonGroupAdorner GetAdorner => this.groupAdorner;
    }
}

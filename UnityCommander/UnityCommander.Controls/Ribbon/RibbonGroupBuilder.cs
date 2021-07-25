
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;

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
            this.group = new RibbonGroup();
            return this;
        }

        /// <summary>
        /// The add button.
        /// </summary>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonGroup"/>.
        /// </returns>
        public RibbonGroupBuilder AddButton(IIcon icon, ICommand command)
        {
            RibbonItem item = new RibbonItem(new (icon, command));
            this.group.Children.Add(item);
            return this;
        }

        /// <summary>
        /// The get group.
        /// </summary>
        /// <returns>
        /// The <see cref="RibbonGroup"/>.
        /// </returns>
        internal RibbonGroup GetGroup() => this.group;
    }
}

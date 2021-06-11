
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts.Columns;

    /// <summary>
    /// The unity controls.
    /// </summary>
    public class UnityContext : RenderTemplate
    {
        /// <summary>
        /// Gets the context menus.
        /// </summary>
        public List<ContextItem> ContextItem { get; internal set; }

        /// <summary>
        /// Gets the control.
        /// </summary>
        public UnityContext Context { get; internal set; }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        public object DataContext { get; internal set; }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public Action Command { get; internal set; }

        /// <summary>
        /// The set render.
        /// </summary>
        /// <param name="render">
        /// The control.
        /// </param>
        /// <returns>
        /// The <see cref="Context"/>>.
        /// </returns>
        internal object SetRender(OptionRender render)
        {
            this.RenderAs = render;
            return this.Context;
        }

        /// <summary>
        /// The add command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal object SetCommand(Action command)
        {
            this.Command = command;
            return this.Context;
        }

        /// <summary>
        /// The add context menu.
        /// </summary>
        /// <param name="menu">
        /// The menu.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal object SetContextMenu(string menu, Action command)
        {
            if (this.ContextItem == null)
            {
                this.ContextItem = new List<ContextItem>();
            }

            this.ContextItem.Add(new ContextItem
                 {
                     Name = menu,
                     Command = command
                 });

            return this.Context;
        }
    }
}

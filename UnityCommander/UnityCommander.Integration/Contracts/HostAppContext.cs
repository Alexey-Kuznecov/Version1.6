
namespace UnityCommander.Integration.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The context host application is an object that allows plugins 
    /// to make changes and expand the functionality of the application.
    /// </summary>
    public class HostAppContext : RenderTemplate
    {
        /// <summary>
        /// Mapping enumeration members to delegates.
        /// </summary>
        private static readonly IDictionary<PluginScopes, Type> AssociateCommands =
            new Dictionary<PluginScopes, Type>
                  {
                      { PluginScopes.Columns, typeof(AddColumnsDelegate) }
        };

        /// <summary>
        /// Gets the list of the context menu item.
        /// </summary>
        public List<ContextItem> ContextItem { get; internal set; }

        /// <summary>
        /// Gets the host application context.
        /// </summary>
        public HostAppContext Context { get; internal set; }

        /// <summary>
        /// Gets the object to deploy to ​the application
        /// </summary>
        public object DataContext { get; internal set; }

        /// <summary>
        /// Gets the command to be called when an event occurs.
        /// </summary>
        public Action Command { get; internal set; }

        /// <summary>
        /// Gets the model that describes the command.
        /// </summary>
        public HostAppCommand CommandModel { get; internal set; }

        /// <summary>
        /// Gets or sets delegate to mapping a command.
        /// </summary>
        public Delegate DelegateCommand { get; set; }

        /// <summary>
        /// Gets the enumeration member that defines the scope of the plugin.
        /// </summary>
        public PluginScopes PluginScope { get; internal set; }

        /// <summary>
        /// Sets the item rendering option based on the selected scope of the plugin.
        /// </summary>
        /// <param name="render">
        /// Specifies the item rendering option.
        /// </param>
        /// <returns>
        /// The <see cref="HostAppContext"/> is automatically created.
        /// </returns>
        internal object SetRender(OptionRender render)
        {
            this.RenderAs = render;
            return this.Context;
        }

        /// <summary>
        /// Sets the command to execute when an event occurs.
        /// </summary>
        /// <param name="command">
        /// Specifies the command to execute,
        /// </param>
        /// <returns>
        /// The <see cref="HostAppContext"/> is automatically created.
        /// </returns>
        internal object SetCommand(Action command)
        {
            this.Command = command;
            return this.Context;
        }

        /// <summary>
        /// Adds context menu item for elements that have a context menu.
        /// </summary>
        /// <param name="menu">
        /// The name of the context menu item.
        /// </param>
        /// <param name="command">
        /// The command to execute when the conetext menu item is selected.
        /// </param>
        /// <returns>
        /// The <see cref="HostAppContext"/> is automatically created.
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

        /// <summary>
        /// Creates and builds new command for data binding to an element.
        /// </summary>
        /// <param name="scopes">
        /// Specifies the scopes of the plugin.
        /// </param>
        /// <param name="commandSource">
        /// Specifies the source of command (the type of object that implemeting the command).
        /// </param>
        /// <param name="command">
        /// Specifies the name of command.
        /// </param>
        /// <returns>
        /// The <see cref="HostAppContext"/> is automatically created.
        /// </returns>
        internal object SetBindingCommand(PluginScopes scopes, Type commandSource, string command)
        {
            this.CommandModel = new HostAppCommand
              {
                  Command = command,
                  SourceCommand = commandSource,
                  DelegateCommand = AssociateCommands.Single(k => k.Key == scopes).Value
              };
            return this.Context;
        }
    }
}

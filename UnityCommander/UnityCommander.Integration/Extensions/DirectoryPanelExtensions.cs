
namespace UnityCommander.Integration.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// Provides extension methods for creating a host application context..
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public static class DirectoryPanelExtensions
    {
        /// <summary>
        /// Adds a new column for folder/file.
        /// </summary>
        /// <param name="scopes">
        /// The plugin scopes.
        /// </param>
        /// <param name="target">
        /// The target panel is used only for the split panel.
        /// </param>
        /// <param name="header">
        /// The column header.
        /// </param>
        /// <param name="width">
        /// The column width.
        /// </param>
        /// <returns>
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </returns>
        public static PluginBuilder Add(this PluginScopes scopes, TargetPanel target, string header, int width)
        {
            switch (scopes)
            {
                case PluginScopes.Columns:
                    var unity = new PluginBuilder();
                    unity.DataContext = new Column { Header = header, Width = width, TargetPanel = target };
                    unity.Builder = unity;
                    unity.PluginScope = scopes;
                    return unity.Builder;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Provides data binding to a element.
        /// </summary>
        /// <param name="context">
        /// The host application context is automatically created.   
        /// </param>
        /// <param name="source">
        /// The source of the command, usually a class that implements 
        /// <see cref="IPluginImplement"/>, but you can specify any class.
        /// </param>
        /// <param name="command">
        /// The name of the command. This method will be called whenever required.
        /// </param>
        /// <returns>
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </returns>
        public static PluginBuilder AddBindingCommand(this PluginBuilder context, Type source, string command)
        {
            context.SetBindingCommand(context.PluginScope, source, command);
            return context;
        }

        /// <summary>
        /// Specifies the method for rendering a element selected from the <see cref="OptionRender"/> list.
        /// </summary>
        /// <param name="context">
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </param>
        /// <param name="method">
        /// Method rendering a element selected from the <see cref="OptionRender"/> list.
        /// </param>
        /// <returns>
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </returns>
        public static PluginBuilder AddRender(this PluginBuilder context, OptionRender method)
        {
            return (PluginBuilder)context.SetRender(method);
        }

        /// <summary>
        /// Sets the command to call each time one of the events occurs. 
        /// See the documentation to see what events can be associated with commands.
        /// </summary>
        /// <param name="context">
        ///  The host app context.
        /// </param>
        /// <param name="command">
        /// The command or method that will be called when the event occurs.
        /// </param>
        /// <returns>
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </returns>
        public static PluginBuilder AddCommand(this PluginBuilder context, Action command)
        {
            return (PluginBuilder)context.SetCommand(command);
        }

        /// <summary>
        /// Adds a context menu item for elements that have a context menu.
        /// </summary>
        /// <param name="context">
        /// The host app context.
        /// </param>
        /// <param name="name">
        /// Specifies the name of the context menu item.
        /// </param>
        /// <param name="command">
        /// Specifies the command to call when a context menu item is selected.
        /// </param>
        /// <returns>
        /// The <see cref="PluginBuilder"/> is automatically created.
        /// </returns>
        public static PluginBuilder AddContextItem(this PluginBuilder context, string name, Action command)
        {
            return (PluginBuilder)context.SetContextMenu(name, command);
        }
    }
}

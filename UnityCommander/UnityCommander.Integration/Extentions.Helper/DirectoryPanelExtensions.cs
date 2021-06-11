
namespace UnityCommander.Integration.Extentions.Helper
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The directory panel extensions.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public static class DirectoryPanelExtensions
    {
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="scopes">
        /// The scopes.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public static UnityContext Add(this PluginScopes scopes, string header, int width)
        {
            switch (scopes)
            {
                case PluginScopes.Columns:
                    var unity = new UnityContext();
                    unity.DataContext = new Column { Header = header, Width = width };
                    unity.Context = unity;
                    return scopes == PluginScopes.Columns ? unity.Context : null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// The set render.
        /// </summary>
        /// <param name="context">
        /// The render.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public static UnityContext AddRender(this UnityContext context, OptionRender template)
        {
            return (UnityContext)context.SetRender(template);
        }

        /// <summary>
        /// The set action.
        /// </summary>
        /// <param name="context">
        /// The column.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public static UnityContext AddCommand(this UnityContext context, Action command)
        {
            return (UnityContext)context.SetCommand(command);
        }

        /// <summary>
        /// The set context menu.
        /// </summary>
        /// <param name="context">
        /// The context menu.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="command">
        /// The command list.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public static UnityContext AddContextItem(this UnityContext context, string header, Action command)
        {
            return (UnityContext)context.SetContextMenu(header, command);
        }

        /// <summary>
        /// The set sort command.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="headerList">
        /// The header list.
        /// </param>
        /// <param name="commandList">
        /// The command list.
        /// </param>
        /// <returns>
        /// The <see cref="Column"/>.
        /// </returns>
        public static UnityContext SetSortCommand(this UnityContext column, string[] headerList, Action commandList)
        {
            return column;
        }
    }
}

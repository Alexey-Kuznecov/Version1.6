
namespace UnityCommander.Integration.Columns
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The column builder.
    /// </summary>
    public class ColumnBuilder
    {
        /// <summary>
        /// The columns.
        /// </summary>
        private readonly List<Column> columns = new ();

        /// <summary>
        /// The column.
        /// </summary>
        private Column column;

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public void Add(string header, double width)
        {
            this.column = new Column
            {
                Header = header,
                Width = width
            };

            this.columns.Add(this.column);
        }

        /// <summary>
        /// The add command.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void AddCommand(Action action)
        {
            this.column.SortCommand = action;
        }

        /// <summary>
        /// The binding option.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <param name="render">
        /// The render.
        /// </param>
        public void BindingOption(Type source, string propertyName, Selector handler, OptionRender render = OptionRender.Default)
        {
        }

        /// <summary>
        /// The add context item.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void AddContextItem(string header, Action action)
        {
            this.column.ContextItems.Add(new ContextItem
            {
                Name = header,
                Command = action
            });
        }

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <returns>
        /// The list of columns.
        /// </returns>
        public List<Column> GetColumns()
        {
            return this.columns;
        }
    }
}

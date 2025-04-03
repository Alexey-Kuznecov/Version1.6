

namespace UnityCommander.Integration.Columns
{
    using System;
    using System.Collections.Generic;
    using UnityCommander.Common;
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;


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
        /// The add context item.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void AddContextItem(string header, Action<string> action)
        {
            var type = action.Method.DeclaringType;

            this.column.ContextItems.Add(new ContextItem
            {
                Name = header,
                Command = new GlobalCommand
                {
                    DisplayName = header,
                    Command = new GlobalCommandExecute(action, type)
                }
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

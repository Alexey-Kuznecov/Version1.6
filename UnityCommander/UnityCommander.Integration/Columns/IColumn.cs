
namespace UnityCommander.Integration.Columns
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Common;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The interface column.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets or sets the column header.
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        public object Template { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets a command to sort values in a column.
        /// </summary>
        public Action SortCommand { get; set; }

        /// <summary>
        /// Gets or sets context menu item for a column.
        /// </summary>
        public List<ContextItem> ContextItems { get; set; }

        /// <summary>
        /// Gets or sets the option builders.
        /// </summary>
        public List<OptionBuilder> OptionBuilders { get; set; }

        /// <summary>
        /// Gets or sets the column builder.
        /// </summary>
        public IColumnBuilder ColumnBuilder { get; set; }

        /// <summary>
        /// Gets or sets the column manager.
        /// </summary>
        public ColumnManager ColumnManager { get; set; }
    }
}

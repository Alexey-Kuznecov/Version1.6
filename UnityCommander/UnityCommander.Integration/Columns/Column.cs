
namespace UnityCommander.Integration.Columns
{
    using System;
    using System.Collections.Generic;

    using Contracts;

    using UnityCommander.Integration.Options;

    /// <summary>
    /// The column.
    /// </summary>
    public class Column : IColumn
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

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
        public List<ContextItem> ContextItems { get; set; } = new ();

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        public object Template { get; set; }

        /// <summary>
        /// Gets or sets the option builders.
        /// </summary>
        public List<OptionBuilder> OptionBuilders { get; set; } = new ();
    }
}

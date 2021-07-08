
using System;

namespace UnityCommander.Integration.Columns
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The column.
    /// </summary>
    public class Column
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
        /// Gets or sets the width.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }

        /// <summary>
        /// Gets or sets a command to sort values in a column.
        /// </summary>
        public Action SortCommand { get; set; }

        /// <summary>
        /// Gets or sets context menu item for a column.
        /// </summary>
        public List<ContextItem> ContextItems { get; set; } = new();
    }
}

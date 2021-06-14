
namespace UnityCommander.Integration.Contracts.Columns
{
    using System;

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
    }
}


namespace DateTimeColumns
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The image column.
    /// </summary>
    public class DateTimeColumnModel : IColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether is displayed.
        /// </summary>
        public bool IsDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public object Template { get; set; }

        /// <summary>
        /// Gets or sets the target panel.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the sort command.
        /// </summary>
        public Action SortCommand { get; set; }

        /// <summary>
        /// Gets or sets the context items.
        /// </summary>
        public List<ContextItem> ContextItems { get; set; }

        /// <summary>
        /// Gets or sets the option builders.
        /// </summary>
        public List<OptionBuilder> OptionBuilders { get; set; }
    }
}

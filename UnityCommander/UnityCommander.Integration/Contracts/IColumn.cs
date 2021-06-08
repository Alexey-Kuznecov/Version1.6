
using UnityCommander.Integration.Enums;

namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The i columns.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether
        /// the column is installed in the directory panel.
        /// </summary>
        bool IsDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the column header.
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        object Template { get; set; }

        /// <summary>
        /// Gets or sets the target panel for which the plug-in is intended, such as a folder panel or a file panel.
        /// This property is typically used for a split panel.
        /// </summary>
        TargetPanel TargetPanel { get; set; }
    }
}

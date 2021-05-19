
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The i columns.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether
        /// the column is installed in the directory pane.
        /// </summary>
        bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets the column header.
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        object ColumnTemplate { get; set; }
    }
}


namespace UnityCommander.Common.Models
{
    using System.Windows.Controls;
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The column model.
    /// </summary>
    public class ColumnModel : IColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether is checked.
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the column view.
        /// </summary>
        public object ColumnTemplate { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public InjectData Action { get; set; }
    }
}

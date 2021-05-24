
namespace UnityCommander.Common.Models.Base
{
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The column model.
    /// </summary>
    public class BaseColumn : IColumn
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
        public object Template { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public InjectData Action { get; set; }
    }
}

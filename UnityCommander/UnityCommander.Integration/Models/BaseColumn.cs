
namespace UnityCommander.Integration.Models
{
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The basic properties that are required for
    /// correct interaction between the model and the view model.
    /// </summary>
    public class BaseColumn : IColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether the column will be displayed.
        /// </summary>
        public bool IsDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the column header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        public object Template { get; set; }

        /// <summary>
        /// Gets or sets the action that is always executed when complete the column.
        /// </summary>
        public InjectData Action { get; set; }
    }
}


namespace ImagesColumns
{
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The image column.
    /// </summary>
    public class ImageColumnModel : IColumn
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
        /// Gets or sets target panel.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }
    }
}

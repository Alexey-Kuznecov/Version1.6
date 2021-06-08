
namespace DateTimeColumns
{
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
        public TargetPanel TargetPanel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}

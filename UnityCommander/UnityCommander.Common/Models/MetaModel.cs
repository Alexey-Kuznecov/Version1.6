
namespace UnityCommander.Common.Models
{
    using System.Windows;
    using UnityCommander.Common.Enums;

    /// <summary>
    /// The meta model.
    /// </summary>
    public class MetaModel
    {
        /// <summary>
        /// Gets or sets the column template.
        /// </summary>
        public DataTemplate ColumnTemplate { get; set; }

        /// <summary>
        /// Gets or sets the directory item's type.
        /// </summary>
        public DirectoryItemType Type { get; set; }
    }
}


namespace UnityCommander.Common.Models
{
    using System;
    using System.Windows;
    using UnityCommander.Common.Enums;

    /// <summary>
    /// The meta model.
    /// </summary>
    [Serializable]
    public class MetaModel
    {
        /// <summary>
        /// Gets or sets the directory item's type.
        /// </summary>
        public DirectoryItemType Type { get; set; }
    }
}

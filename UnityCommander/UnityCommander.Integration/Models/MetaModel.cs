
namespace UnityCommander.Integration.Models
{
    using System;

    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The meta model.
    /// </summary>
    [Serializable]
    public class MetaModel
    {
        /// <summary>
        /// Gets or sets the directory item's type.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }
    }
}

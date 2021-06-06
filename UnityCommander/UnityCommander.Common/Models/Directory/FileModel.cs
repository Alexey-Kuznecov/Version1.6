
namespace UnityCommander.Common.Models.Directory
{
    using System;

    using UnityCommander.Integration.Models.Base;

    /// <summary>
    /// The file model.
    /// </summary>
    [Serializable]
    public class FileModel : BaseDirectory
    {
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get; set; }
    }
}

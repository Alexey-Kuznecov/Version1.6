
namespace UnityCommander.Common.Models.Directory
{
    using System;

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

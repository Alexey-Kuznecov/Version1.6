
namespace UnityCommander.Integration.Models.Base
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The directory base.
    /// </summary>
    [Serializable]
    public abstract class BaseDirectory : MetaModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public IconModel Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time the file/folder was created.
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time the file/folder was last accessed.
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets additional columns that are provided by plugin.
        /// </summary>
        public Dictionary<string, object> Additional { get; set; } = new();
    }
}

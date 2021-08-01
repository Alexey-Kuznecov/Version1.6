
namespace UnityCommander.Common.Models.Directory
{
    using System;
    using System.Collections.Generic;
    using NSwag.Collections;

    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The directory base.
    /// </summary>
    [Serializable]
    public abstract class BaseDirectory
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
        public Icon Icon { get; set; }

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
        public IDictionary<string, object> Additional { get; set; } = new ObservableDictionary<string, object>();

        /// <summary>
        /// Gets or sets the directory item's type.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }
    }
}
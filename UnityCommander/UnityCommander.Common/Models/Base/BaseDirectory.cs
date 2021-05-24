
namespace UnityCommander.Common.Models.Base
{
    using System;

    /// <summary>
    /// The directory base.
    /// </summary>
    public class BaseDirectory : MetaModel
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
    }
}

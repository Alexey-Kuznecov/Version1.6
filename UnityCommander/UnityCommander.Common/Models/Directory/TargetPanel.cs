
namespace UnityCommander.Common.Models.Directory
{
    using System;

    /// <summary>
    /// The directory item type.
    /// </summary>
    [Flags]
    public enum TargetPanel
    {
        All,
        /// <summary>
        /// File.
        /// </summary>
        Files,

        /// <summary>
        /// Folder.
        /// </summary>
        Folders,

        /// <summary>
        /// Local disk.
        /// </summary>
        LocalDisk
    }
}

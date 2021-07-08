
namespace UnityCommander.Integration.Enums
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
        /// The files.
        /// </summary>
        Files,

        /// <summary>
        /// The folders.
        /// </summary>
        Folders
    }
}

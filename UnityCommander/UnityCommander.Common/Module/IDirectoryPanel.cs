
namespace UnityCommander.Common.Module
{
    using System;

    /// <summary>
    /// The DirectoryPanel interface.
    /// </summary>
    public interface IDirectoryPanel : ITabPanelContent
    {
        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public string GetCurrentPath();
    }
}

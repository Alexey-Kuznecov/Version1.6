
namespace UnityCommander.Core.Modules
{
    using System;

    /// <summary>
    /// The DirectoryPanel interface.
    /// </summary>
    public interface IDirectoryPanel
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the   panel.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryPanel"/>.
        /// </returns>
        public IDirectoryPanel InitializedViewModel(Guid token, string path);
        
        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken();

        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public string GetCurrentPath();
    }
}


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
        public void InitializedViewModel();
        
        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken();

        /// <summary>
        /// The get instance number.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public byte GetInstanceNumber();
    }
}

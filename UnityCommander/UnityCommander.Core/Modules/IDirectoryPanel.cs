
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
        /// Gets or sets the initial command.
        /// </summary>
        /// <param name="panelToken">
        /// The command manager.
        /// </param>
        public void InitialPanel(Guid panelToken);
    }
}

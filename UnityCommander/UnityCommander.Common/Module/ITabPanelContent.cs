
namespace UnityCommander.Common.Module
{
    using System;

    /// <summary>
    /// The TabPanelContent interface.
    /// </summary>
    public interface ITabPanelContent
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken();

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
        /// The <see cref="ITabPanelContent"/>.
        /// </returns>
        public ITabPanelContent InitializedViewModel(Guid token, string path);
    }
}

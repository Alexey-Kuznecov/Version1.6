
namespace UnityCommander.Common.Module
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The TabPanelContent interface.
    /// </summary>
    public interface ITabPanelContent
    {
        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken();

        /// <summary>
        /// The initialized view model.
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
        public ITabPanelContent InitializedViewModel(ref Guid token, string path);

        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public string GetCurrentPath();
    }
}

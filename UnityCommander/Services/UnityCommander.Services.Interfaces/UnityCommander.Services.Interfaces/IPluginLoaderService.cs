
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// Plugin provider service interface.
    /// </summary>
    public interface IPluginLoaderService
    {
        /// <summary>
        /// The unload plugins.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool UnloadPlugins();

        /// <summary>
        /// The create plugin context.
        /// </summary>
        void CreatePluginContext();

        /// <summary>
        /// Gets the interfaces to implement the custom dialog of the host application.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        IEnumerable<IDialogService> GetDialogService();

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginContext"/>.
        /// </returns>
        IEnumerable<IPluginContext> GetPluginContext();

        /// <summary>
        /// The get plugin descriptors.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginDescriptor"/>.
        /// </returns>
        IEnumerable<IPluginDescriptor> GetPluginDescriptors();
    }
}

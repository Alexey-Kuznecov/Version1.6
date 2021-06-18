
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// Plugin provider service interface.
    /// </summary>
    public interface IPluginLoaderService
    {
        /// <summary>
        /// Gets the interfaces to manage plugin settings.
        /// </summary>
        IEnumerable<IPluginConfigure> ImportPluginSettings { get; }

        /// <summary>
        /// Gets the plugins implementation.
        /// </summary>
        IEnumerable<IPluginImplement> ImportPluginImplements { get; }

        /// <summary>
        /// Gets the dialog service to create owner dialog box.
        /// </summary>
        IEnumerable<IDialogService> ImportDialogService { get; }

        /// <summary>
        /// Gets a list of plugins that implement the <see cref="IPluginImplement"/> interface.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginImplement"/>.
        /// </returns>
        IEnumerable<IPluginImplement> GetPluginImplements();

        /// <summary>
        /// Gets the interfaces to implement the custom dialog of the host application.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogService();

        /// <summary>
        /// Gets list interfaces to manage plugins is imported.
        /// </summary>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// List of plugins that implement the specified interface.
        /// </returns>
        public IEnumerable<T> GetPluginContract<T>();

        /// <summary>
        /// Gets an instance of the class that implements the plugin interface.
        /// </summary>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// List class instances.
        /// </returns>
        public IEnumerable<object> GetPluginInstances<T>();
    }
}

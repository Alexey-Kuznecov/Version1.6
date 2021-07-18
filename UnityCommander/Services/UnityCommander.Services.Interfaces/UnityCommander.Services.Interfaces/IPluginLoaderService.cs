
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
        IEnumerable<IDialogService> GetDialogService();

        /// <summary>
        /// The get column builders.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IColumnBuilder"/>.
        /// </returns>
        IEnumerable<IColumnBuilder> GetColumnBuilders();

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginContext"/>.
        /// </returns>
        IEnumerable<IPluginContext> GetPluginContext();

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The interface <see cref="IColumnBuilder"/>.
        /// </returns>
        IPluginContext GetPluginContext(int index);

        /// <summary>
        /// The create plugin context.
        /// </summary>
        void CreatePluginContext();

        /// <summary>
        /// The get plugin descriptors.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginDescriptor"/>.
        /// </returns>
        IEnumerable<IPluginDescriptor> GetPluginDescriptors();

        /// <summary>
        /// Gets list interfaces to manage plugins is imported.
        /// </summary>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// List of plugins that implement the specified interface.
        /// </returns>
        IEnumerable<T> GetPluginContract<T>();

        /// <summary>
        /// Gets an instance of the class that implements the plugin interface.
        /// </summary>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// List class instances.
        /// </returns>
        IEnumerable<object> GetPluginInstances<T>();
    }
}

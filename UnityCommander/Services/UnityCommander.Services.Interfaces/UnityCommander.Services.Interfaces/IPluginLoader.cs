
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;

    /// <summary>
    /// The PluginLoader interface.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// The unload plugin.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UnloadPlugin();

        /// <summary>
        /// The get implements.
        /// </summary>
        /// <returns>
        /// The interface plugin implement.
        /// </returns>
        public IEnumerable<IPluginImplement> GetImplements();

        /// <summary>
        /// The get configurations.
        /// </summary>
        /// <returns>
        /// The interface plugin configure.
        /// </returns>
        public IEnumerable<IPluginConfigure> GetConfigurations();

        /// <summary>
        /// The get descriptors.
        /// </summary>
        /// <returns>
        /// The interface plugin descriptor.
        /// </returns>
        public IEnumerable<IPluginDescriptor> GetDescriptors();

        /// <summary>
        /// The get dialogs.
        /// </summary>
        /// <returns>
        /// The interface dialog service.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogs();

        /// <summary>
        /// The get column builders.
        /// </summary>
        /// <returns>
        /// The interface column builder.
        /// </returns>
        public IEnumerable<IColumnBuilder> GetColumnBuilders();
    }
}

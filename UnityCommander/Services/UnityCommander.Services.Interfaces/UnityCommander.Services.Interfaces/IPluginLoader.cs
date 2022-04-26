
using UnityCommander.Integration.Commands;

namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using Integration.Columns;
    using Integration.Contracts;
    using Integration.Dialog;
    using Integration.Options;

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

        /// <summary>
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<IOptionBuilder> GetOptionBuilders();

        /// <summary>
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<BaseCommand> GetPluginCommands();
    }
}

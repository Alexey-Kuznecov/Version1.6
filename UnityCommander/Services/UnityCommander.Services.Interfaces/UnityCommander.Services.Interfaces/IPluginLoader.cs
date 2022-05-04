
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using Integration.Columns;
    using Integration.Contracts;
    using Integration.Dialog;
    using Integration.Options;
    using UnityCommander.Integration.Commands;

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
        /// Получает коллекцию загружаемых служб, которые были найдены в подключаемых модулях.
        /// </summary>
        /// <typeparam name="T">
        /// Тип службы которую нужна получить.
        /// </typeparam>
        /// <returns>
        /// Возвращает коллекцию загружаемых служб.
        /// </returns>
        public IEnumerable<T> GetServices<T>() where T : IPluginService;

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
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<BaseCommand> GetPluginCommands();
    }
}

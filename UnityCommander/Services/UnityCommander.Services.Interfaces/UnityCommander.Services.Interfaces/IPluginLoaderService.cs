
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;

    /// <summary>
    /// The PluginProviderService interface.
    /// </summary>
    public interface IPluginLoaderService
    {
        /// <summary>
        /// Gets or sets the import plugin factories.
        /// </summary>
        IEnumerable<IPluginConfigure> ImportPluginSettings { get; set; }

        /// <summary>
        /// Gets or sets the import plugin imlementations.
        /// </summary>
        IEnumerable<IPluginImplements> ImportPluginImplements { get; set; }

        /// <summary>
        /// Gets or sets the import columns services.
        /// </summary>
        IEnumerable<IColumnService> ImportColumnServices { get; set; }

        /// <summary>
        /// The get column service.
        /// </summary>
        /// <returns>
        /// The <see cref="IPluginImplements"/>.
        /// </returns>
        IEnumerable<IPluginImplements> GetPluginImplements();

        /// <summary>
        /// The get column service.
        /// </summary>
        /// <typeparam name="T">
        /// The contract type
        /// </typeparam>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public IEnumerable<T> GetPluginContract<T>();

        /// <summary>
        /// The get plugin settings.
        /// </summary>
        /// <returns>
        /// The <see cref="IPluginConfigure"/>.
        /// </returns>
        IEnumerable<IPluginConfigure> GetPluginSettings();


        /// <summary>
        /// The get plugin settings.
        /// </summary>
        /// <returns>
        /// The <see cref="IColumnService"/>.
        /// </returns>`
        IEnumerable<IColumnService> GetColumnService();
    }
}

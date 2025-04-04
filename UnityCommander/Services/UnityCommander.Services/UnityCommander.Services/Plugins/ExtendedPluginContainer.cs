using System.Runtime.Loader;
using UnityCommander.Integration;
using UnityCommander.Integration.Contracts;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Plugins
{
    /// <summary>
    /// Расширенный контейнер для плагина, включающий загрузчик плагина.
    /// </summary>
    public class ExtendedPluginContainer : PluginContainer
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ExtendedPluginContainer"/>.
        /// </summary>
        /// <param name="pluginName">Имя плагина.</param>
        /// <param name="assemblyPath">Полный путь к сборке плагина.</param>
        /// <param name="loadContext">Контекст загрузки плагина.</param>
        /// <param name="pluginContext">Контекст плагина.</param>
        /// <param name="pluginLoader">Загрузчик плагина.</param>
        public ExtendedPluginContainer(
            string pluginName,
            string assemblyPath,
            AssemblyLoadContext loadContext,
            IPluginContext pluginContext,
            IPluginLoader pluginLoader)
            : base(pluginName, assemblyPath, loadContext, pluginContext)
        {
            PluginLoader = pluginLoader;
        }

        /// <summary>
        /// Получает загрузчик плагина.
        /// </summary>
        public IPluginLoader PluginLoader { get; }


        /// <summary>
        /// Выгружает плагин.
        /// </summary>
        public void Unload()
        {
            PluginLoader.Unload();
        }
    }
}

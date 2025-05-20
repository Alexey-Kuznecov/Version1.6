using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// Контейнер для плагина, включая его метаданные, путь и контекст.
    /// </summary>
    public class PluginContainer
    {
        /// <summary>
        /// Имя плагина.
        /// </summary>
        public string PluginName { get; init; }

        /// <summary>
        /// Полный путь к сборке плагина (DLL).
        /// </summary>
        public string AssemblyPath { get; init; }

        /// <summary>
        /// Контекст загрузки плагина.
        /// </summary>
        public AssemblyLoadContext LoadContext { get; init; }

        /// <summary>
        /// Контекст плагина, описывающий его функциональность.
        /// </summary>
        public IPluginContext PluginContext { get; init; }

        public PluginContainer(
            string pluginName,
            string assemblyPath,
            AssemblyLoadContext loadContext,
            IPluginContext pluginContext)
        {
            this.PluginName = pluginName;
            this.AssemblyPath = assemblyPath;
            this.LoadContext = loadContext;
            this.PluginContext = pluginContext;
        }

        /// <summary>
        /// Возращает имя плагина что также является именем файла (.dll)
        /// </summary>
        /// <returns></returns>
        public string GetPluginName() => this.PluginName;

        /// <summary>
        /// Возращает полный путь к плагина.
        /// </summary>
        /// <returns></returns>
        public string GetPluginPath() => this.AssemblyPath;
    }
}

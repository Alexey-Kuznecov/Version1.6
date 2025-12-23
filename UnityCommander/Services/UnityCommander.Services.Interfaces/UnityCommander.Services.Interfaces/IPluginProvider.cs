
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces
{
    /// <summary>
    /// The PluginProvider interface.
    /// </summary>
    public interface IPluginProvider
    {
        IReadOnlyCollection<PluginInfo> Plugins { get; }

        IReadOnlyCollection<PluginInfo> LoadedPlugins { get; }

        bool Load(string idOrPath);

        IEnumerable<IPluginContainer> LoadAll();
        IEnumerable<IPluginContainer> GetAll();

        bool Unload(string pluginId);

        void UnloadAll();

        PluginInfo? GetInfo(string pluginId);
    }
}

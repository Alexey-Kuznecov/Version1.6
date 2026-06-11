
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces.Plugins
{
    /// <summary>
    /// The PluginProvider interface.
    /// </summary>
    public interface IPluginInfoProvider
    {
        IReadOnlyCollection<PluginInfo> Plugins { get; }

        PluginInfo? GetInfo(string pluginId);
        
        void LoadMetadata();
    }
}

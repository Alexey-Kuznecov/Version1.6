

using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginCatalog
    {
        void LoadMetadata();
        IReadOnlyCollection<PluginInfo> GetAll();
        PluginInfo Get(string id);
    }
}

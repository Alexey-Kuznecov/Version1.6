
using System.Collections.Generic;
using UnityCommander.Common.Plugins;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginSettingsProvider
    {
        IEnumerable<PluginSettingsDescriptor>
            GetSettings(string pluginId);
    }
}

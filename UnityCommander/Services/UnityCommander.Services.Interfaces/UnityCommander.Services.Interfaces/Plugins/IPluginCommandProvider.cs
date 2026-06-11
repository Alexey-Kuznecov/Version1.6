
using System.Collections.Generic;
using UnityCommander.Common.Plugins;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginCommandProvider
    {
        IEnumerable<PluginCommandDescriptor>
            GetCommands(string pluginId);
    }
}

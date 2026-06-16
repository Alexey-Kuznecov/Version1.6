
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginCommandProvider
    {
        IEnumerable<PluginCommandDescriptor>
            GetCommands(string pluginId);
    }
}

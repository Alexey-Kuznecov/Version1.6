

using UnityCommander.Common.Plugins;

namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarDefinition : IPluginOwned
    {
        string Id { get; }

        string Category { get; }

        string IconKey { get; }

        string ViewKey { get; }
    }
}

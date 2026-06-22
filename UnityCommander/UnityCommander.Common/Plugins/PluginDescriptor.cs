

using System.Windows.Media;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Common.Plugins
{
    public class PluginDescriptor
    {
        public string Id { get; init; }

        public Geometry Icon { get; init; }

        public string IconKey { get; init; }

        public string DisplayName { get; init; }

        public string Version { get; init; }

        public string Author { get; init; }

        public string Description { get; init; }

        public bool IsLoaded { get; init; }
    }
}

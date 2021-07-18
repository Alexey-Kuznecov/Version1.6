using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Contracts;

namespace W3Manager.WP2
{
    class PluginDescription : IPluginDescriptor
    {
        public string DisplayName { get; set; } = "Mod columns";
        public string Description { get; set; } = "Columns for your mod.";
    }
}

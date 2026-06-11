

using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using UnityCommander.Common.Sidebar;

[assembly: PluginInfo(
    name: "PluginTest",
    developerId: "plugin-test-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин расширяет сайдбар новыми секциями"
)]
namespace PluginTest
{
    public class Plugin : IPlugin
    {
        public string Name => "Plugin Test";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            init.RegisterSidebarItem(
                new SidebarSection(
                    "plugin-test-1.0",
                    "Sack",
                    typeof(SidebarSectionTreeView),
                    typeof(SidebarSectionTree)));
        }

        public void Start(IPluginContext context)
        {
            //throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}

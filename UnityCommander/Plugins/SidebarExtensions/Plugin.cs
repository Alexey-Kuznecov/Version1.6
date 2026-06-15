
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using SidebarExtensions;
using UnityCommander.Common.Sidebar;

[assembly: PluginInfo(
    name: "Sidebar Extensions Plugin",
    developerId: "sidebar-ex-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин расширяет сайдбар новыми секциями"
)]
namespace MultiColumns
{
    public class Plugin : IPlugin
    {
        public string Name => "Sidebar Extensions Plugin";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            init.RegisterSidebarItem(
                new SidebarSection(
                    "sidebar-ex-1.0",
                    "Git",
                    typeof(SidebarGitView), 
                    typeof(SidebarGitViewModel)));

            init.RegisterView<SidebarGitView, SidebarGitViewModel>();
        }

        public void Start(IPluginContext context)
        {
            //throw new System.NotImplementedException();
        }

        public void Stop()
        {
        }
    }
}

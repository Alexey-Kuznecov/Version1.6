
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;

[assembly: PluginInfo(
    name: "W3Manager Plugin",
    developerId: "w3Manager-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин расширяет сайдбар новыми секциями"
)]
namespace W3Manager
{
    public class Plugin : IPlugin
    {
        public string Name => "W3Manager Plugin";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            init.RegisterView<UserControl1, SidebarGitViewModel>();
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

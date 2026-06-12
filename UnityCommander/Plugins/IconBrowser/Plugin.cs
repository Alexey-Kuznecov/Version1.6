
using IconBrowser;
using IconBrowser.ViewModels;
using IconBrowser.Views;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using UnityCommander.Common.Dialog;

[assembly: PluginInfo(
    name: "Icon Maker Plugin",
    developerId: "icon-maker-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Плагин для создания иконок"
)]
namespace IconBrowser
{
    public class Plugin : IPlugin
    {
        public string Name => "icon_maker";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            //init.RegisterView<IconBrowserControl, IconBrowserViewModel>();
            //init.RegisterView<IconMakerView, IconMakerViewModel>();
            init.RegisterDialog(
                new DialogDefinition(
                    "icon_maker-1.0",
                    typeof(IconBrowserControl), 
                    typeof(IconBrowserViewModel)
            ));
            init.RegisterDialog(
                 new DialogDefinition(
                     "icon_maker-1.0-new",
                     typeof(IconMakerView),
                     typeof(IconMakerViewModel)
         ));
        }

        public void Start(IPluginContext context)
        {

        }

        public void Stop()
        {
        }
    }
}

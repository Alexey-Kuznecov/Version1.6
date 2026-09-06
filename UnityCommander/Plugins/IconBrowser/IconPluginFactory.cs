
using PluginSystem.Abstractions.Plugin;

namespace IconBrowser
{
    internal class IconPluginFactory : IPluginFactory
    {
        public IPlugin Create()
        {
            return new Plugin();
        }

        public PluginInfo GetPluginInfo(PluginInfo info)
        {
            info.Name = "Icon Maker Plugin";
            info.Version = "1.0";
            info.Author = "UnityCommander Team";
            info.DeveloperID = "icon-maker-1.0";

            return info;
        }
    }
}

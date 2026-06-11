
namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginActivator
    {
        public void Activate(string pluginId);

        public void ActivateStartupPlugins();
    }
}

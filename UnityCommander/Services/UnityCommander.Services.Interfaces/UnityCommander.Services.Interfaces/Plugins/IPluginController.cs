
namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginController
    {
        bool Load(string id);

        bool Unload(string id);

        void LoadAll();

        void UnloadAll();
    }
}

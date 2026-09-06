
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Abstractions.Statusbar
{
    public interface IStatusBarRegistry : IOwnedRegistry
    {
        event Action<string>? OwnerUnload;

        void Register(IStatusBarItem statusBarItem);

        IStatusBarItem? Get(string id);

        IEnumerable<IStatusBarItem> GetAll();

        void Unregister(string id);
    }
}

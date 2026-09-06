
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Abstractions.Statusbar
{
    public interface IStatusBarProvider
    {
        IEnumerable<IStatusBarItem> CreateItems();
    }
}

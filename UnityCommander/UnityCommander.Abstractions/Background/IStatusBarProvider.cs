
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Abstractions.Background
{
    public interface IStatusBarProvider
    {
        IEnumerable<IStatusBarItem> GetItems();
    }
}

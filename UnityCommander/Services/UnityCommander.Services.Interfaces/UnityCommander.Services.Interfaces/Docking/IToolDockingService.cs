
using AvalonDock;

namespace UnityCommander.Services.Interfaces.Docking
{
    public interface IToolDockingService
    {
        void SetDockingManager(DockingManager manager);

        void Load();
        void Save();
    }
}

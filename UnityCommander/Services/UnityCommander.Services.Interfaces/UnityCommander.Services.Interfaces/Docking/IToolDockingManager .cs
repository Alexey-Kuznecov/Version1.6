
namespace UnityCommander.Services.Interfaces.Docking
{
    public interface IToolDockingManager
    {
        void Create(IToolDescriptor descriptor);

        void Remove(string toolId);
    }
}

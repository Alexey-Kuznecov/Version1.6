
namespace UnityCommander.Abstractions.Sidebar
{
    public interface ISidebarRegistry : IOwnedRegistry
    {
        event Action<string>? OwnerUnload;

        void Register(ISidebarSection section);

        ISidebarSection? Get(string id);

        IEnumerable<ISidebarSection> GetAll();

        void Unregister(string id);
    }
}

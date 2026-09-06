
namespace UnityCommander.Abstractions.Sidebar
{
    public interface ISidebarSectionFactory
    {
        ISidebarSection Create(ISidebarDefinition definition);
    }
}

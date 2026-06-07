
namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarSectionFactory
    {
        ISidebarSection Create(ISidebarDefinition definition);
    }
}

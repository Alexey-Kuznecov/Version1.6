

namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarDefinition
    {
        string Id { get; }

        string Category { get; }

        string IconKey { get; }

        string ViewKey { get; }
    }
}

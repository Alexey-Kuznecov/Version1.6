

namespace UnityCommander.Abstractions.Sidebar
{
    public interface ISidebarDefinition : IOwned
    {
        string Id { get; }

        string Category { get; }

        string IconKey { get; }

        string ViewKey { get; }
    }
}

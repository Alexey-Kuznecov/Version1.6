
namespace UnityCommander.Abstractions.Panels
{
    public class TabRemovedEvent
    {
        public Guid PanelId { get; init; }

        public Guid TabId { get; init; }
    }
}

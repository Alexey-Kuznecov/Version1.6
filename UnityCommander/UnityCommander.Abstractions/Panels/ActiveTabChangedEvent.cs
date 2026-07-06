
namespace UnityCommander.Abstractions.Panels
{
    public class ActiveTabChangedEvent
    {
        public Guid PanelId { get; init; }

        public Guid TabId { get; init; }
    }
}

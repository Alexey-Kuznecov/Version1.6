using System;

namespace UnityCommander.Abstractions.Panels
{
    public class TabAddedEvent
    {
        public Guid PanelId { get; init; }

        public Guid TabId { get; init; }
    }
}
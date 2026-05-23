using System;

namespace UnityCommander.Common.Panels.Panels
{
    public class TabAddedEvent
    {
        public Guid PanelId { get; init; }

        public Guid TabId { get; init; }
    }
}
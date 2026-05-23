
using System;

namespace UnityCommander.Common.Docking
{
    public class TabOperation
    {
        public TabOperationType Type { get; init; }

        public Guid TabId { get; init; }

        public Guid? FromPanelId { get; init; }
        public Guid? ToPanelId { get; init; }

        public string? Path { get; init; } // если нужно при создании
    }
}

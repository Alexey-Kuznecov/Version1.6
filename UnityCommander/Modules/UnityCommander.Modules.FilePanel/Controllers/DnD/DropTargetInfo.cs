
using System;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class DropTargetInfo
    {
        public string? Path { get; init; }
        public Guid? TabId { get; init; }
    }
}

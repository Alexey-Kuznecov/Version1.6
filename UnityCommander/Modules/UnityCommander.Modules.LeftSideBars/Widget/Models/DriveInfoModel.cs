
using UnityCommander.UI.Interaction;

namespace UnityCommander.Modules.LeftSideBars.Widget.Models
{
    public sealed class DriveInfoModel : IInteractionContextSource
    {
        public string Letter { get; init; } = "";
        public string Name { get; init; } = "";

        public string IconKey { get; init; }

        public long TotalSize { get; init; }
        public long AvailableFreeSpace { get; init; }

        public long UsedSize =>
            TotalSize - AvailableFreeSpace;

        public double UsedRatio =>
            TotalSize > 0
                ? (double)UsedSize / TotalSize
                : 0;

        public IInteractionContext? GetContext()
        {
            return new DiskContext(Letter);
        }
    }
}

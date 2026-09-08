
namespace UnityCommander.Modules.LeftSideBars.Widget.Models
{
    public sealed class DriveInfoModel
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
    }
}

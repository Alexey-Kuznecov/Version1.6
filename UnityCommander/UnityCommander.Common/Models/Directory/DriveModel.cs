
namespace UnityCommander.Common.Models.Directory
{
    public class DriveModel
    {
        public string Letter { get; set; }

        public string IconKey { get; set; }

        public long TotalAmount { get; set; }

        public long FreeSpace { get; set; }

        public long UsedSpace { get; set; }

        public TargetPanel TargetPanel { get; set; }
    }
}

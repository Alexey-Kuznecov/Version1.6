
namespace UnityCommander.Common.Models.Directory
{
    using UnityCommander.Common.Models.Icons;

    public class DriveModel
    {
        public string Letter { get; set; }

        public IIcon Icon { get; set; }

        public string IconKey { get; set; }

        public long TotalAmount { get; set; }

        public long FreeSpace { get; set; }

        public long UsedSpace { get; set; }

        public TargetPanel TargetPanel { get; set; }
    }
}

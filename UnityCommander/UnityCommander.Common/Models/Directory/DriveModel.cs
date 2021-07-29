
namespace UnityCommander.Common.Models.Directory
{
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The model describes a local disk.
    /// </summary>
    public class DriveModel
    {
        /// <summary>
        /// Gets or sets the drive letter.
        /// </summary>
        public string Letter { get; set; }

        /// <summary>
        /// Gets or sets the drive icon.
        /// </summary>
        public IIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the total disk space.
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the available disk space.
        /// </summary>
        public double FreeSpace { get; set; }

        /// <summary>
        /// Gets or sets the byte space to be occupied.
        /// </summary>
        public double UsedSpace { get; set; }

        /// <summary>
        /// Gets or sets the target panel.
        /// </summary>
        public TargetPanel TargetPanel { get; set; }
    }
}

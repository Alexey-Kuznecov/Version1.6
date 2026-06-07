
using UnityCommander.Controls.Layout;

namespace UnityCommander.Modules.FilePanel.States
{
    public class Workspace
    {
        public RegionNode HeaderRegion { get; }
        public RegionNode MainRegion { get; }
        public RegionNode SecondaryRegion { get; }

        public Workspace(
            RegionNode headerRegion,
            RegionNode mainRegion,
            RegionNode secondaryRegion)
        {
            HeaderRegion = headerRegion;
            MainRegion = mainRegion;
            SecondaryRegion = secondaryRegion;
        }
    }
}

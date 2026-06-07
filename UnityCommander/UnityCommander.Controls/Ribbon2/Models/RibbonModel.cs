
using System.Collections.Generic;

namespace UnityCommander.Controls.Ribbon2.Models
{
    public class RibbonModel
    {
        public List<RibbonTabModel> Tabs { get; set; } = new();
        public RibbonLayoutConfig LayoutConfig { get; set; } = new();
    }
}

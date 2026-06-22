
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class RibbonButton : RibbonControl
    {
        public RibbonButton(string buttonName, IIcon buttonIcon, RibbonCommand buttonCommand)
            : base(
                buttonName, 
                buttonIcon, 
                buttonCommand, 
                "RibbonButtonStyles",
                "RibbonButtonTemplate",
                string.Empty)
        {
        }
    }
}

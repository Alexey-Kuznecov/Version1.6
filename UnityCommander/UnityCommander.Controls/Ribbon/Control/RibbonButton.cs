
namespace UnityCommander.Controls.Ribbon.Control
{
    using UnityCommander.Common.Models.Icons;

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

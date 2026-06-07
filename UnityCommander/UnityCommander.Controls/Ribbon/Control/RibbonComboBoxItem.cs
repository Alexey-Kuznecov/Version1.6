
namespace UnityCommander.Controls.Ribbon.Control
{
    using UnityCommander.Common.Models.Icons;

    public class RibbonComboBoxItem : RibbonControl
    {
        public RibbonComboBoxItem(string content, IIcon buttonIcon, RibbonCommand buttonCommand)
            : base(
                content, 
                buttonIcon, 
                buttonCommand, 
                "RibbonComboBoxItemStyles", 
                "RibbonComboBoxItemTemplate", 
                "RibbonComboBoxItemDataTemplate")
        {
        }
    }
}

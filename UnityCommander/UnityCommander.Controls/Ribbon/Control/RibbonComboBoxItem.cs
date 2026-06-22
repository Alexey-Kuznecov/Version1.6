
namespace UnityCommander.Controls.Ribbon.Control
{
    using UnityCommander.Rendering.Icons;

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


namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Controls;

    using UnityCommander.Controls.Ribbon.Control;

    public class RibbonElement : ContentControl
    {
        private readonly ContentControl contentControl = new ();
        public RibbonElement(IRibbonControl item)
        {
            this.contentControl.DataContext = item.DataBinding;
            this.contentControl.Template = item.Template;
            this.contentControl.Style = item.Style;
            this.Content = this.contentControl;
        }
    }
}

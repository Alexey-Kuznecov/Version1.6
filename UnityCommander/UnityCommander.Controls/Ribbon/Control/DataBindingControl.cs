
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows.Shapes;
    public class DataBindingControl
    {
        public object Content { get;set; }
        public Path Icon { get; set; }
        public RibbonCommand GlobalCommand { get; set; }
    }
}

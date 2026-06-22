
using System.Windows.Input;
using System.Windows.Shapes;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class DropListPopupModel
    {
        public ICommand Command { get; set; }
        public Path Icon { get; set; }
        public object Content { get; set; }
    }
}


using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IconMaker.Core.Models
{
    [Serializable]
    public class ButtonExtension
    {
        public ushort Id { get; set; }

        public DrawingBrush Brush { get; set; }

        public ButtonExtension CommandParameter { get; set; }

        public SolidColorBrush Color { get; set; }

        public Style Style { get; set; }

        public Path Path { get; set; }

        public string CollectionName { get; set; }

        public string IconName { get; set; }

        public object ToolTip { get; set; }

        public object Template { get; set; }
              
        public ICommand RemoveIcon { get; set; }

        public ICommand RenameIcon { get; set; }

        public ICommand ReplaceIcon { get; set; }
    }
}

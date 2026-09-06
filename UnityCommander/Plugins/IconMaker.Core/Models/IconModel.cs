
using System.Windows.Media;
using System.Windows.Shapes;

namespace IconMaker.Core.Models
{
    public class IconModel
    {
        #region Properties

        public ushort Id { get; set; }
        public string StringPath { get; set; }
        public string Name { get; set; }
        public string CollectionName { get; set; }
        public int Scale { get; set; }
        public DrawingBrush Brush { get; set; }
        public SolidColorBrush BgroundColor { get; set; }
        public SolidColorBrush FgroundColor { get; set; }
        public Path Path { get; set; }
        public List<Path> PathList { get; set; }

        #endregion
    }
}

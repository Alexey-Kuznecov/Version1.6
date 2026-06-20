
namespace UnityCommander.Common.Models.Icons
{
    using System;
    using System.Windows.Media;
    using System.Windows.Shapes;

    [Serializable]
    public class Icon : IIcon
    {
        public string Name { get; set; }

        public DrawingBrush Brush { get; set; }

        public Path Path { get; set; }

        public string Category { get; set; }

        public Path GetIconPath()
        {
            return Path;
        }
    }
}

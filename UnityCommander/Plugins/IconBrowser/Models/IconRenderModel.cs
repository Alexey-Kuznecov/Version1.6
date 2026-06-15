
using System;
using System.Windows.Media;

namespace IconBrowser.Models
{
    public sealed class IconRenderModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Geometry[] Geometry { get; set; }

        public Brush Brush { get; set; } // runtime only
    }
}


using System.Windows.Media;

namespace IconMaker.Core.Models
{
    public interface IColorScheme
    {
        public string Name { get; set; }

        public Color Primary { get; set; }
        public Color Secondary { get; set; }
        public Color Accent { get; set; }
    }
}
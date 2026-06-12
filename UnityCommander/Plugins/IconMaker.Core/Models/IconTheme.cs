
using System.Windows.Media;

namespace IconMaker.Core.Models
{
    public sealed class IconTheme
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string PackId { get; set; }

        public double Scale { get; set; }

        public string ColorSchemeId { get; set; }

        public bool IsMonochrome { get; set; }
    }
}

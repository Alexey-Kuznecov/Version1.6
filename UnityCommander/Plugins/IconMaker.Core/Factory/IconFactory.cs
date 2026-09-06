
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
using System.Windows.Shapes;

namespace IconMaker.Core.Factory
{
    public class IconFactory : IIconFactory
    {
        public IconModel Create()
        {
            throw new NotImplementedException();
        }

        public IconModel CreateFromButton(ButtonExtension button)
        {
            return new IconModel
            {
                Name = button?.IconName,
                Path = button?.Path,
                Brush = button?.Brush,
                FgroundColor = "#FFFFFF".StringFormatToSolidColor(),
            };
        }

        public IconModel CreateFromPaths(
            string name,
            string collection,
            List<Path> paths,
            string backgroundColor)
        {
            return new IconModel
            {
                Name = name,
                CollectionName = collection,
                PathList = paths,
                Scale = 64,
                FgroundColor = "#FFFFFF".StringFormatToSolidColor(),
                BgroundColor = backgroundColor.StringFormatToSolidColor()
            };
        }
    }
}

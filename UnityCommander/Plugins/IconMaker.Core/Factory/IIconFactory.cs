
using IconMaker.Core.Models;
using System.Windows.Shapes;

namespace IconMaker.Core.Factory
{
    public interface IIconFactory
    {
        IconModel Create();

        IconModel CreateFromButton(ButtonExtension button);

        IconModel CreateFromPaths(
            string name,
            string collection,
            List<Path> paths,
            string backgroundColor);
    }
}
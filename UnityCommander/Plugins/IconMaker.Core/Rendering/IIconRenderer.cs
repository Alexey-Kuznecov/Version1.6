using IconMaker.Core.Models;

namespace IconMaker.Core.Rendering
{
    public interface IIconRenderer
    {
        object Render(
            IconDefinition icon,
            RenderOptions options);
    }
}

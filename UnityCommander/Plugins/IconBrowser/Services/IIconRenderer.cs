
using IconBrowser.Models;
using IconMaker.Core.Models;

namespace IconBrowser.Services
{
    public interface IIconRenderer
    {
        IconRenderModel Build(IconDefinition icon, IconTheme theme);
    }
}

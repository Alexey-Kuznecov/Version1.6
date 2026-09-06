
using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconProvider
    {
        IconDefinition? GetIcon(
            string iconName,
            IconTarget target);
    }
}

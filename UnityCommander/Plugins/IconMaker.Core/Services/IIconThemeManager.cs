
using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconThemeManager
    {
        IconTheme? GetTheme(IconTarget target);

        void SetTheme(
            IconTarget target,
            IconTheme theme);

        void RemoveTheme(IconTarget target);
    }
}


using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconThemeRuntime
    {
        IconTheme CurrentTheme { get; }

        void Apply(string themeId);
    }
}

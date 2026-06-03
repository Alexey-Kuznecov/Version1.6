
namespace UnityCommander.Core.Theming
{
    public interface IThemeCatalog
    {
        ThemeDefinition Get(string name);

        ThemeDefinition Default { get; }
    }
}

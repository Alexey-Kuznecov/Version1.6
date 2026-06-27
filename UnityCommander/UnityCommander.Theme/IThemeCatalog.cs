
namespace UnityCommander.Theme
{
    public interface IThemeCatalog
    {
        ThemeDefinition Get(string name);

        ThemeDefinition Default { get; }
    }
}

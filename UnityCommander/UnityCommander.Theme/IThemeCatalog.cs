
namespace UnityCommander.Theme
{
    public interface IThemeCatalog
    {
        ThemeDefinition Get(string name);

        ThemeDefinition LightTheme { get; }

        ThemeDefinition DarkTheme { get; }
    }
}


using UnityCommander.Abstractions.Settings;
using UnityCommander.Settings.Core;

namespace UnityCommander.Settings
{
    public sealed class UiSettingsProvider : ISettingsProvider
    {
        public IEnumerable<SettingDefinition> GetDefinitions()
        {
            yield return GeneralSettings.ShowHiddenFiles;
        }
    }
}

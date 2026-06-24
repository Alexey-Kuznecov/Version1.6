
using System.Collections.Generic;
using UnityCommander.Abstractions.Settings;
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    public sealed class ShortcutSettingsProvider
     : ISettingsProvider
    {
        public IEnumerable<SettingDefinition> GetDefinitions()
        {
            yield return ShortcutSettings.ToggleSidebar;
            yield return ShortcutSettings.ToggleRibbon;
            yield return ShortcutSettings.ToggleConsole;
            //yield return ShortcutSettings.ShowSettings;
        }
    }
}

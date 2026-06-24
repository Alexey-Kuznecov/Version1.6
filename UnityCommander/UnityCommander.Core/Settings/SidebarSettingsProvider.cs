
using System.Collections.Generic;
using UnityCommander.Abstractions.Settings;
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    public sealed class UiSettingsProvider : ISettingsProvider
    {
        public IEnumerable<SettingDefinition> GetDefinitions()
        {
            yield return UiSettings.SidebarPosition;
        }
    }
}

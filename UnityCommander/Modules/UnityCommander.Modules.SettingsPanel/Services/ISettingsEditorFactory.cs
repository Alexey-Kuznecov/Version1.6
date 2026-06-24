

using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.Settings.Core;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public interface ISettingsEditorFactory
    {
        SettingItemViewModel Create(SettingDefinition def, object value);
    }
}
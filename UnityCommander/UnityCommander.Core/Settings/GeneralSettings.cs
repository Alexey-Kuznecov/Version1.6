
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    public static class GeneralSettings
    {
        public static readonly SettingDefinition<bool>
            ShowHiddenFiles =
                SettingDefinitionFactory.Create(
                    "files.showHidden",
                    "Показывать скрытые файлы",
                    "Отображает скрытые и системные файлы",
                    "Files",
                    false);
    }
}


using UnityCommander.Settings.Core;

namespace UnityCommander.Settings
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

        public static readonly SettingDefinition<bool>
            ShowCopyProgressDialog =
                SettingDefinitionFactory.Create(
                    "files.copy.showProgressDialog",
                    "Показывать окно прогресса копирования",
                    "Если отключено, прогресс отображается только в файловых панелях.",
                    "Files",
                    true);
    }
}

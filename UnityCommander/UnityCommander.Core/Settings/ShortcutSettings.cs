
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Common.Commands;
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    public static class ShortcutSettings
    {
        public static readonly SettingDefinition<ShortcutOverride>
            ShowSettings =
                SettingDefinitionFactory.Create(
                    "shortcuts.showSettings",
                    "Показать настройки",
                    "Открыть диалоговое окно настроек",
                    "Shortcuts",
                    new ShortcutOverride
                    {
                        CommandId = CommandNames.UI.ShowSettings,
                        Key = ShortcutKey.F12,
                        Modifiers = ShortcutModifiers.None
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleSidebar =
                SettingDefinitionFactory.Create(
                    "shortcuts.toggleSidebar",
                    "Переключить сайдбар",
                    "Показывает или скрывает боковою панель",
                    "Shortcuts",
                    new ShortcutOverride
                    { 
                        CommandId = CommandNames.UI.ToggleSidebar,
                        Key = ShortcutKey.R, 
                        Modifiers = ShortcutModifiers.Ctrl 
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleRibbon =
                SettingDefinitionFactory.Create(
                    "shortcuts.toggleRibbon",
                    "Переключить ленту",
                    "Показывает или скрывает ленту",
                    "Shortcuts",
                    new ShortcutOverride
                    {
                        CommandId = CommandNames.UI.ToggleRibbon,
                        Key = ShortcutKey.V,
                        Modifiers = ShortcutModifiers.Ctrl
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleConsole =
                SettingDefinitionFactory.Create(
                    "shortcuts.toggleConsole",
                    "Скрыть\\Показать консоль",
                    "Показывает или скрывает консоль",
                    "Shortcuts",
                      new ShortcutOverride
                      {
                          CommandId = CommandNames.UI.ToggleBottomPanel,
                          Key = ShortcutKey.Oem3,
                          Modifiers = ShortcutModifiers.Ctrl
                      });
    }
}

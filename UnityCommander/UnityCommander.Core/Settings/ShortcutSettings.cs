
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Common.Commands;
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    public static class ShortcutSettings
    {
        public static readonly SettingDefinition<ShortcutOverride>
            ShowSettings =
                ShotcutBindingFactory.Create(
                    "shortcuts.showSettings",
                    new ShortcutOverride
                    {
                        CommandId = CommandNames.UI.ShowSettings,
                        Key = ShortcutKey.F12,
                        Modifiers = ShortcutModifiers.None
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleSidebar =
                ShotcutBindingFactory.Create(
                    "shortcuts.toggleSidebar",
                    new ShortcutOverride
                    { 
                        CommandId = CommandNames.UI.ToggleSidebar,
                        Key = ShortcutKey.R, 
                        Modifiers = ShortcutModifiers.Ctrl 
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleRibbon =
                ShotcutBindingFactory.Create(
                    "shortcuts.toggleRibbon",
                    new ShortcutOverride
                    {
                        CommandId = CommandNames.UI.ToggleRibbon,
                        Key = ShortcutKey.V,
                        Modifiers = ShortcutModifiers.Ctrl
                    });

        public static readonly SettingDefinition<ShortcutOverride>
            ToggleConsole =
                ShotcutBindingFactory.Create(
                    "shortcuts.toggleConsole",
                      new ShortcutOverride
                      {
                          CommandId = CommandNames.UI.ToggleBottomPanel,
                          Key = ShortcutKey.Oem3,
                          Modifiers = ShortcutModifiers.Ctrl
                      });
    }
}

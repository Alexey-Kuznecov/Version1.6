
using Prism.Ioc;
using System.IO;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Abstractions.Settings;
using UnityCommander.Common;
using UnityCommander.Core.Keyboad;
using UnityCommander.Core.Registrar;
using UnityCommander.Modules.SettingsPanel.Services;
using UnityCommander.Settings;
using UnityCommander.Settings.Abstactions;
using UnityCommander.Settings.Core;
using UnityCommander.WPF.Input;

namespace UnityCommander.Dependencies
{
    public static class SettingsRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IShortcutContextService, ShortcutContextService>();
            registry.RegisterSingleton<IShortcutResolver, ShortcutResolver>();
            registry.RegisterSingleton<IShortcutRegistry, ShortcutRegistry>();
            registry.RegisterSingleton<IInputService, InputService>();

            registry.RegisterSingleton<IShortcutOverrideStore, ShortcutOverrideStore>();
            registry.RegisterSingleton<IShortcutMapProvider, ShortcutMapProvider>();
            registry.RegisterSingleton<IShortcutMapBuilder, ShortcutMapBuilder>();
            registry.RegisterSingleton<JsonShortcutOverrideStorage>(sp =>
            {
                var paths = sp.Resolve<UnityCommanderPath>();

                return new JsonShortcutOverrideStorage(
                    paths.Config("shortcuts.json"));
            });

            registry.RegisterSingleton<ISettingsService, SettingsService>();

            registry.RegisterSingleton<ISettingsStore>(sp =>
            {
                var paths = sp.Resolve<UnityCommanderPath>();

                return new JsonSettingsStore(
                    paths.Config("settings.json"));
            });

            //registry.RegisterSingleton<ISettingsProvider, ShortcutSettingsProvider>();
            registry.RegisterSingleton<ISettingsProvider, UiSettingsProvider>();

            registry.RegisterSingleton<ISettingsViewModelBuilder, SettingsViewModelBuilder>();
            registry.RegisterSingleton<ISettingsSectionProvider, SettingsSectionProvider>();
            registry.RegisterSingleton<ISettingsSectionProvider, SortcutSectionProvider>();
            registry.RegisterSingleton<ISettingsEditorFactory, SettingsEditorFactory>();
        }
    }
}

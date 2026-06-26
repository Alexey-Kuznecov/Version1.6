
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.Editors;
using UnityCommander.Modules.SettingsPanel.ViewModels;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class SortcutSectionProvider : ISettingsSectionProvider
    {
        private readonly IShortcutRegistry _shortcuts;
        private readonly IShortcutOverrideStore _shortcutStore;
        private readonly IServiceProvider _sp;

        public SortcutSectionProvider(
            IShortcutRegistry shortcuts,
            IShortcutOverrideStore shortcutStore, 
            IServiceProvider sp)
        {
            _sp = sp;
            _shortcuts = shortcuts;
            _shortcutStore = shortcutStore;
        }

        public IEnumerable<SettingsPageViewModel> BuildPages()
        {
            var page = new SettingsPageViewModel
            {
                Title = "Shortcuts"
            };

            foreach (var shortcut in _shortcuts.GetAll())
            {
                _shortcutStore.TryGet(shortcut.CommandId, out var overrided);

                var def = shortcut;

                var item = CreateShortcut(def, overrided);

                page.Items.Add(item);
            }

            yield return page;
        }

        private SettingItemViewModel CreateShortcut(ShortcutDefinition def, ShortcutOverride value)
        {
            var vm = ActivatorUtilities.CreateInstance<ShortcutEditorViewModel>(_sp);

            vm.Definition = def;
            vm.Value = value;

            return new SettingItemViewModel
            {
                Key = def.Key,
                Description = def.Description,
                Value = value,
                Editor = new ShortcutEditor
                {
                    DataContext = vm
                }
            };
        }
    }
}

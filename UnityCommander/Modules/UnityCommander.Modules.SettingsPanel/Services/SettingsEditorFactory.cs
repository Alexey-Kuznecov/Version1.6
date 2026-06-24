
using Microsoft.Extensions.DependencyInjection;
using System;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.Editors;
using UnityCommander.Modules.SettingsPanel.ViewModels;
using UnityCommander.Settings.Core;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class SettingsEditorFactory : ISettingsEditorFactory
    {
        private readonly IServiceProvider _sp;

        public SettingsEditorFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public SettingItemViewModel Create(SettingDefinition def, object value)
        {
            return def.ValueType switch
            {
                var t when t == typeof(bool)
                    => CreateBool(def, (bool)value),

                var t when t == typeof(string)
                    => CreateString(def, (string)value),

                var t when t == typeof(ShortcutOverride)
                    => CreateShortcut(def, (ShortcutOverride)value),

                _ => throw new Exception("Unknown editor")
            };
        }

        private SettingItemViewModel CreateShortcut(SettingDefinition def, ShortcutOverride value)
        {
            var vm = ActivatorUtilities.CreateInstance<ShortcutEditorViewModel>(_sp);

            vm.Value = value;

            return new SettingItemViewModel
            {
                Key = def.Key,
                Title = def.DisplayName,
                Category = def.Category,
                Description = def.Description,
                Value = value,
                Editor = new ShortcutEditor
                {
                    DataContext = vm
                }
            };
        }

        private SettingItemViewModel CreateString(SettingDefinition def, string value)
        {
            return new SettingItemViewModel
            {
                Key = def.Key,
                Title = def.DisplayName,
                Category = def.Category,
                Description = def.Description,
                Value = value,
                Editor = new TextEditor
                {
                    DataContext = value
                }
            };
        }

        private SettingItemViewModel CreateBool(SettingDefinition def, bool value)
        {
            return new SettingItemViewModel
            {
                Key = def.Key,
                Title = def.DisplayName,
                Category = def.Category,
                Description = def.Description,
                Value = value,
                Editor = new BoolEditor
                {
                    DataContext = value
                }
            };
        }
    }
}

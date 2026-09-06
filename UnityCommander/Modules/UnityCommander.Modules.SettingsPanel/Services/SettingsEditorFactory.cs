
using Microsoft.Extensions.DependencyInjection;
using System;
using UnityCommander.Modules.SettingsPanel.Editors;
using UnityCommander.Modules.SettingsPanel.Models;
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

                _ => throw new Exception("Unknown editor")
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

        private SettingItemViewModel CreateBool(
            SettingDefinition def,
            bool value)
        {
            var vm = ActivatorUtilities.CreateInstance<BoolEditorViewModel>(_sp);

            vm.Entry = new SettingEntry<bool>(def, value);
            vm.Value = value;

            return new SettingItemViewModel
            {
                Key = def.Key,
                Title = def.DisplayName,
                Category = def.Category,
                Description = def.Description,
                Value = value,
                Editor = new BoolEditor
                {
                    DataContext = vm
                }
            };
        }
    }
}

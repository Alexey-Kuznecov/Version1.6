
using UnityCommander.Settings.Core;

namespace UnityCommander.Modules.SettingsPanel.Models
{
    public sealed class SettingEntry<T>
    {
        public SettingDefinition Definition { get; }

        public T Value { get; set; }

        public SettingEntry(SettingDefinition definition, T value)
        {
            Definition = definition;
            Value = value;
        }
    }
}

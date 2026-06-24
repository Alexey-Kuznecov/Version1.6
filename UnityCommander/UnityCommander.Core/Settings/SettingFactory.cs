using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    internal class ShotcutBindingFactory
    {
        internal static SettingDefinition<T> Create<T>(string name, T defaultValue)
        {
            return new SettingDefinition<T>()
            {
                Category = "Shortcuts",
                DisplayName = name,
                Description = "Переопределние горячих клавиш",
                Key = name,
                ValueType = typeof(T),
                DefaultValue = defaultValue
            };
        }
    }
}
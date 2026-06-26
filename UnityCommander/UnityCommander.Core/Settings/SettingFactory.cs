
using UnityCommander.Settings.Core;

namespace UnityCommander.Core.Settings
{
    internal class SettingDefinitionFactory
    {
        internal static SettingDefinition<T> Create<T>(
            string key,
            string displayName,
            string description,
            string category,
            T defaultValue)
        {
            return new()
            {
                Key = key,
                DisplayName = displayName,
                Description = description,
                Category = category,
                ValueType = typeof(T),
                DefaultValue = defaultValue
            };
        }
    }
}
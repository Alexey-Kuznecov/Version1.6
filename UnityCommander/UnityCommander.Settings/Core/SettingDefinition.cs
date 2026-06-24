
namespace UnityCommander.Settings.Core
{
    public abstract class SettingDefinition
    {
        public required SettingsKey Key { get; init; }

        public required Type ValueType { get; init; }

        public required object DefaultValue { get; init; }

        public string? Category { get; init; }

        public string? Description { get; init; }

        public string? DisplayName { get; set; }
    }

    public sealed class SettingDefinition<T>
        : SettingDefinition
    {
    }
}
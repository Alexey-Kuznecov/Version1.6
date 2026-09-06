
namespace UnityCommander.Settings.Core
{
    public readonly record struct SettingsKey(string Value)
    {
        public override string ToString() => Value;

        public static implicit operator string(SettingsKey key) => key.Value;
        public static implicit operator SettingsKey(string key) => new(key);
    }
}

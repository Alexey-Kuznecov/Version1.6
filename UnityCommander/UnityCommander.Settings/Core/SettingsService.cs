
using UnityCommander.Abstractions.Settings;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Settings.Core
{
    public sealed class SettingsService : ISettingsService
    {
        private readonly ISettingsStore _store;

        private readonly Dictionary<string, SettingDefinition> _definitions;

        private Dictionary<string, object?> _values;

        public SettingsService(
            ISettingsStore store,
            IEnumerable<ISettingsProvider> providers)
        {
            _store = store;

            _values = _store.Load();

            _definitions = providers
                .SelectMany(p => p.GetDefinitions())
                .ToDictionary(x => x.Key.Value);

            foreach (var def in _definitions)
            {
                if (!_values.ContainsKey(def.Key))
                    _values[def.Key] = def.Value.DefaultValue;
            }
        }

        public IEnumerable<SettingDefinition> GetDefinitions()
            => _definitions.Values;

        public IEnumerable<SettingDefinition<T>> GetDefinitions<T>()
            => _definitions.Values
                    .OfType<SettingDefinition<T>>();

        public T Get<T>(SettingDefinition<T> definition)
        {
            if (_values.TryGetValue(definition.Key, out var value))
                return (T)value!;

            throw new Exception("Value missing (bootstrap broken)");
        }

        public object Get(SettingDefinition def)
        {
            if (_values.TryGetValue(def.Key, out var value))
                return value!;

            return def.DefaultValue;
        }

        public void Set<T>(SettingDefinition<T> definition, T value)
        {
            _values[definition.Key] = value;
            _store.Save(_values);
        }

        public void Set(SettingDefinition definition, object value)
        {
            _values[definition.Key] = value;
            _store.Save(_values);
        }

        public void Reset<T>(SettingDefinition<T> definition)
        {
            _values.Remove(definition.Key);
            _store.Save(_values);
        }
    }
}

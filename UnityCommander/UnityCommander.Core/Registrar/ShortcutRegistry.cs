
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Registrar
{
    public sealed class ShortcutRegistry : IShortcutRegistry
    {
        private readonly Dictionary<string, ShortcutDefinition> _map =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, ShortcutOverride> _overrides =
            new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyCollection<ShortcutDefinition> GetAll()
            => _map.Values;

        public void Register(ShortcutDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);
            ArgumentException.ThrowIfNullOrWhiteSpace(definition.CommandId);

            _map[definition.CommandId] = definition;
        }

        public bool TryGet(string commandId, out ShortcutDefinition definition)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandId);

            return _map.TryGetValue(commandId, out definition!);
        }

        public bool Remove(string commandId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandId);

            return _map.Remove(commandId);
        }

        public void Clear()
        {
            _map.Clear();
        }
    }
}

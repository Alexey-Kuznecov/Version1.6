
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Plugin;
using UnityCommander.Abstractions.Plugins;

namespace UnityCommander.Core.Plugin
{
    public class CompositionRegistry : ICompositionRegistry
    {
        private readonly Dictionary<Type, CompositionDefinition> _byType = new();

        private readonly Dictionary<string, CompositionDefinition> _byId = new();

        public void Register(CompositionDefinition definition)
        {
            _byType[definition.WindowType] = definition;
            _byId[definition.Id] = definition;
        }

        public CompositionDefinition Get(Type windowType)
            => _byType[windowType];

        public CompositionDefinition Get(string id)
            => _byId[id];

        public bool TryGet(Type windowType,
            out CompositionDefinition definition)
        {
            return _byType.TryGetValue(
                windowType,
                out definition);
        }

        public bool TryGet(string id,
            out CompositionDefinition definition)
        {
            return _byId.TryGetValue(
                id,
                out definition);
        }

        public void Cleanup(string ownerId)
        {
            throw new NotImplementedException();
        }
    }
}

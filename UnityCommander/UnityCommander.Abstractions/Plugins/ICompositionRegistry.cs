using UnityCommander.Abstractions.Plugin;

namespace UnityCommander.Abstractions.Plugins
{
    public interface ICompositionRegistry : IOwnedRegistry
    {
        void Register(CompositionDefinition definition);
        
        CompositionDefinition Get(Type windowType);

        bool TryGet(Type windowType, out CompositionDefinition def);

        bool TryGet(string id, out CompositionDefinition def);
    }
}


using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public sealed class SearchCommandDefinition : SimpleCommandDescriptor
    {
        public SearchCommandDefinition()
           : base(name: "search",
                variants: Array.Empty<CommandVariant>(),
                arguments: new IPositionalArgumentDescriptor[]
                {
                     new SimplePositionalArgumentDescriptor(
                        name: "source",
                        valueType: ArgumentValueType.Path,
                        isRequired: true),
                    new SimplePositionalArgumentDescriptor(
                        name: "target",
                        valueType: ArgumentValueType.Path,
                        isRequired: true)
                },
                flags: new IFlagDescriptor[]
                {
                    new SimpleFlagDescriptor(
                        name: "--recursive",
                        shortName: null,
                        requiresValue: false)
                })
            {
        }
    }
}

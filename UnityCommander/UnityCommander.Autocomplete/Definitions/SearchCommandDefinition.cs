
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
                     new PathPositionalArgumentDescriptor(
                        name: "scope",
                        pathKind: PathKind.Directory,
                        isRequired: true),
                      new SimplePositionalArgumentDescriptor(
                        name: "query",
                        valueType: ArgumentValueType.String,
                        isRequired: false)
                },
                flags: new IFlagDescriptor[]
                {
                    new SimpleFlagDescriptor(
                        name: "--extensions",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--created-after",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--created-before",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--modified-before",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--modified-after",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--size-min",
                        shortName: null,
                        requiresValue: true),
                   new SimpleFlagDescriptor(
                        name: "--size-max",
                        shortName: null,
                        requiresValue: true),
                    new SimpleFlagDescriptor(
                        name: "--size",
                        shortName: null,
                        requiresValue: true)
                })
            {
        }
    }
}

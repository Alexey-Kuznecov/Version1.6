
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public sealed class CopyCommandDefinition : SimpleCommandDescriptor
    {
        public CopyCommandDefinition()
           : base(name: "copy",
                variants: Array.Empty<CommandVariant>(),
                arguments: new IPositionalArgumentDescriptor[]
                {
                    new PathPositionalArgumentDescriptor(
                        name: "source",
                        isRequired: true),
                    new PathPositionalArgumentDescriptor(
                        name: "target",
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


using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public sealed class TraceCommandDefinition : SimpleCommandDescriptor
    {
        public TraceCommandDefinition()
           : base(name: "trace",
                variants: Array.Empty<CommandVariant>(),
                arguments: new IPositionalArgumentDescriptor[0],
                flags: new IFlagDescriptor[]
                {
                    new SimpleFlagDescriptor(
                        name: "--source",
                        shortName: null,
                        requiresValue: true, 
                        isRepeatable: false,
                        ArgumentValueType.String),
                    new SimpleFlagDescriptor(
                        name: "--data key=",
                        shortName: null,
                        requiresValue: true,
                        isRepeatable: false,
                        ArgumentValueType.String),
                })
            {
        }
    }
}

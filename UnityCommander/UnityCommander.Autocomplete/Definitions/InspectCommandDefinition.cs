
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public class InspectCommandDefinition : SimpleCommandDescriptor
    {
        public InspectCommandDefinition()
            : base(
                "inspect",
                variants: Array.Empty<CommandVariant>(),
                arguments: new IPositionalArgumentDescriptor[]
                {
                    new SimplePositionalArgumentDescriptor(
                        name: "target",
                        valueType: ArgumentValueType.String,
                        isRequired: true)
                },
                flags: new IFlagDescriptor[]
                {
                    new SimpleFlagDescriptor(
                        name: "--report",
                        shortName: null,
                        requiresValue: false),

                    new SimpleFlagDescriptor(
                        name: "--watch",
                        shortName: null,
                        requiresValue: false),

                    new SimpleFlagDescriptor(
                        name: "--interval",
                        shortName: null,
                        requiresValue: true,
                        valueType: ArgumentValueType.Int,
                        valueSeparator: 
                            ValueSeparator.Equals)
                })
            {
        }
    }
}

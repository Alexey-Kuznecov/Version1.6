
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public class IndexCommandDefinition : SimpleCommandDescriptor
    {
        public IndexCommandDefinition()
            : base(
                "index",
                 variants: new[]
                {
                    new CommandVariant(
                        name: "add",
                        flags: new[]
                            {
                                new SimpleFlagDescriptor(
                                    name: "--recursive",
                                    shortName: null,
                                    requiresValue: false),
                            },
                        arguments: new IPositionalArgumentDescriptor[]
                        {
                            new PathPositionalArgumentDescriptor(
                                name: "path",
                                pathKind: PathKind.Any,
                                isRequired: true)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                        usage: "index add <path>  [--recursive]"),
                        new CommandVariant(
                            name: "get",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new SimplePositionalArgumentDescriptor(
                                    name: "id",
                                    valueType: ArgumentValueType.Int,
                                    isRequired: true)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "index get <id>"),
                        new CommandVariant(
                            name: "update",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new SimplePositionalArgumentDescriptor(
                                    name: "id",
                                    valueType: ArgumentValueType.Int,
                                    isRequired: true)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "index update <id>"),
                        new CommandVariant(
                            name: "delete",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new SimplePositionalArgumentDescriptor(
                                    name: "id",
                                    valueType: ArgumentValueType.Int,
                                    isRequired: true)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "index delete <id>"),
                         new CommandVariant(
                            name: "list",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[0],
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "index list"),
                })
            {
        }
    }
}

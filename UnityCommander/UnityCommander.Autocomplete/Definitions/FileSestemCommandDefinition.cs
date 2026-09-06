
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public class FileSestemCommandDefinition : SimpleCommandDescriptor
    {
        public FileSestemCommandDefinition()
            : base(
                "fs",
                 variants: new[]
                {
                    new CommandVariant(
                        name: "create",
                        flags: new IFlagDescriptor[0],
                        arguments: new IPositionalArgumentDescriptor[]
                        {
                            new PathPositionalArgumentDescriptor(
                                name: "path",
                                pathKind: PathKind.Directory,
                                isRequired: true)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                        usage: "fs create <path>"),
                        
                     new CommandVariant(
                            name: "mkdir",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new PathPositionalArgumentDescriptor(
                                    name: "path",
                                    pathKind: PathKind.Directory,
                                    isRequired: true)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "fs mkdir <path>"),
                        
                     new CommandVariant(
                            name: "delete",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                               new PathPositionalArgumentDescriptor(
                                    name: "path",
                                    pathKind: PathKind.Any,
                                    isRequired: true)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "fs delete <path>"),

                     new CommandVariant(
                            name: "rename",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new PathPositionalArgumentDescriptor(
                                    name: "source",
                                    pathKind: PathKind.Any,
                                    isRequired: true),
                                new PathPositionalArgumentDescriptor(
                                    name: "destination",
                                    pathKind: PathKind.Any,
                                    isRequired: true),
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "fs rename <source> <destination>"),

                     new CommandVariant(
                            name: "list",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                new PathPositionalArgumentDescriptor(
                                    name: "path",
                                    pathKind: PathKind.Directory,
                                    isRequired: true),
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "fs list <path>"),
                          
                     new CommandVariant(
                            name: "stress",
                            flags: new IFlagDescriptor[0],
                            arguments: new IPositionalArgumentDescriptor[]
                            {
                                 new PathPositionalArgumentDescriptor(
                                    name: "path",
                                    pathKind: PathKind.Directory,
                                    isRequired: true),
                                   new SimplePositionalArgumentDescriptor("count", ArgumentValueType.Int)
                            },
                            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                            positionalArgumentPolicy: PositionalArgumentPolicy.AfterVariant,
                            usage: "fs stress <root> <count>"),
                })
            {
        }
    }
}

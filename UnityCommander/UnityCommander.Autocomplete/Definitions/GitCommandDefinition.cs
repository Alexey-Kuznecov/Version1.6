
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public sealed class GitCommandDefinition : SimpleCommandDescriptor
    {
        public GitCommandDefinition()
            : base(
                name: "git",
                variants: new[]
                {
                    new CommandVariant(
                        name: "commit",
                        flags: new[]
                        {
                            new SimpleFlagDescriptor(
                                name: "-m",
                                shortName: null,
                                requiresValue: true,
                                valueType: ArgumentValueType.String),
                            new SimpleFlagDescriptor(
                                name: "--amend",
                                shortName: null,
                                requiresValue: false)
                        },
                        arguments : new List<IPositionalArgumentDescriptor>
                        {
                            new SimplePositionalArgumentDescriptor("message", ArgumentValueType.String)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        usage: "git commit <message> [-m <message>] [--amend]"),
                    new CommandVariant(
                        name: "push",
                        flags: new[]
                        {
                            new SimpleFlagDescriptor(
                                name: "--all",
                                shortName: null,
                                requiresValue: false),

                            new SimpleFlagDescriptor(
                                name: "-a",
                                shortName: null,
                                requiresValue: false,
                                valueType: ArgumentValueType.String)
                            },
                        arguments : new List<IPositionalArgumentDescriptor>
                        {
                            new SimplePositionalArgumentDescriptor("remote", ArgumentValueType.String)
                        },
                        flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                        usage: "git push <message> [-m <message>] [--amend]"
                    )
                })
            {
        }
    }
}

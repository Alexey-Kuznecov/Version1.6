
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Testing.Fake
{
    public static class TestCliInputAnalyzerFactory
    {
        public static ICliInputAnalyzer CreateGitLikeAnalyzer()
        {
            var commands = new List<ICommandDescriptor>
            {
                CreateGitCommand()
            };

            return new DefaultCliInputAnalyzer(commands);
        }

        public static ICommandDescriptor CreateGitCommand()
        {
            return new SimpleCommandDescriptor(
                name: "git",
                variants: new[]
                {
                    CreateGitCommitVariant(),
                    CreateGitPushVariant()
                });
        }

        private static ICommandVariant CreateGitCommitVariant()
        {
            return new CommandVariant(
                name: "commit",
                flags: new[]
                {
            new SimpleFlagDescriptor(
                name: "--amend",
                shortName: null,
                requiresValue: false, 
                isRepeatable: false),

            new SimpleFlagDescriptor(
                name: "-m",
                shortName: null,
                requiresValue: true,
                isRepeatable: true,
                valueType: ArgumentValueType.String)
                },
                arguments: new List<IPositionalArgumentDescriptor>
                {
            new SimplePositionalArgumentDescriptor(
                "message",
                ArgumentValueType.String)
                },
                flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                usage: "git commit <message> [-m <message>] [--amend]"
            );
        }

        private static ICommandVariant CreateGitPushVariant()
        {
            return new CommandVariant(
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
                requiresValue: false)
                },
                arguments: new List<IPositionalArgumentDescriptor>
                {
            new SimplePositionalArgumentDescriptor(
                "remote",
                ArgumentValueType.String)
                },
                flagOrderPolicy: FlagOrderPolicy.StrictOrder,
                usage: "git push <remote> [--all]"
            );
        }
    }
}

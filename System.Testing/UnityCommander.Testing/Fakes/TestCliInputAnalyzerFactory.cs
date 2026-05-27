
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Testing.Fake
{
    public static class TestCliInputAnalyzerFactory
    {
        //public static ICliInputAnalyzer CreateGitLikeAnalyzer()
        //{
        //    var commands = new List<ICommandDescriptor>
        //    {
        //        CreateGitCommand()
        //    };

        //    return new DefaultCliInputAnalyzer(commands);
        //}

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

        public static ICommandDescriptor CreatePluginCommand()
        {
            return new SimpleCommandDescriptor(
               name: "plugin",
               variants: new[]
               {
                  CreatePluginLoadVariant(),
                  CreatePluginUnloadVariant(),
                  CreatePluginReloadVariant(),
                  CreatePluginListVariant(),
                  CreatePluginInfoVariant()
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

        private static ICommandVariant CreatePluginLoadVariant()
        {
            return new CommandVariant(
            name: "load",
            flags: new[]
            {
                new SimpleFlagDescriptor(
                    name: "--force",
                    shortName: "-f",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean),
                new SimpleFlagDescriptor(
                    name: "--dependencies",
                    shortName: "-d",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean)
            },
            arguments: new List<IPositionalArgumentDescriptor>
            {
                new SimplePositionalArgumentDescriptor(
                    name: "path",
                    valueType: ArgumentValueType.Path,
                    isRequired: true)
            },
            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
            usage: "plugin load <path> [--force] [--dependencies]"
            );
        }
        private static ICommandVariant CreatePluginUnloadVariant()
        {
            return new CommandVariant(
            name: "unload",
            flags: new[]
            {
                new SimpleFlagDescriptor(
                    name: "--all",
                    shortName: "-a",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean),
                new SimpleFlagDescriptor(
                    name: "--force",
                    shortName: "-f",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean)
            },
            arguments: new List<IPositionalArgumentDescriptor>
            {
                new SimplePositionalArgumentDescriptor(
                    name: "name",
                    valueType: ArgumentValueType.String,
                    isRequired: false)
            },
            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
            usage: "plugin unload [name] [--all] [--force]"
            );
        }

        private static ICommandVariant CreatePluginReloadVariant()
        {
            return new CommandVariant(
            name: "reload",
            flags: new[]
            {
                new SimpleFlagDescriptor(
                    name: "--all",
                    shortName: "-a",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean)
            },
            arguments: new List<IPositionalArgumentDescriptor>
            {
                new SimplePositionalArgumentDescriptor(
                    name: "name",
                    valueType: ArgumentValueType.String,
                    isRequired: false)
            },
            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
            usage: "plugin reload [name] [--all]"
            );
        }

        private static ICommandVariant CreatePluginListVariant()
        {
            return new CommandVariant(
            name: "list",
            flags: new[]
            {
                new SimpleFlagDescriptor(
                    name: "--verbose",
                    shortName: "-v",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean)
            },
            arguments: new List<IPositionalArgumentDescriptor>(), // путь не нужен для list
            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
            usage: "plugin list [--verbose]"
            );
        }

        private static ICommandVariant CreatePluginInfoVariant()
        {
            return new CommandVariant(
            name: "info",
            flags: new[]
            {
                new SimpleFlagDescriptor(
                    name: "--all",
                    shortName: "-a",
                    requiresValue: false,
                    valueType: ArgumentValueType.Boolean)
            },
            arguments: new List<IPositionalArgumentDescriptor>
            {
                new SimplePositionalArgumentDescriptor(
                    name: "name",
                    valueType: ArgumentValueType.String,
                    isRequired: true)
            },
            flagOrderPolicy: FlagOrderPolicy.StrictOrder,
            usage: "plugin info <name> [--all]"
            );
        }
    }
}

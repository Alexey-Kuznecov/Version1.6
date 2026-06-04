
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Definitions
{
    public sealed class PluginCommandDefinition : SimpleCommandDescriptor
    {
        public PluginCommandDefinition()
            : base(name: "plugin",
               variants: new[]
               {
                   // ─── Load ─────────────────────
                   new CommandVariant(
                       name: "load",
                       flags: new[]
                       {
                           new SimpleFlagDescriptor(
                               name: "--force",
                               shortName: "-f",
                               requiresValue: true,
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
                   ),

                   // ─── Unload ─────────────────────
                   new CommandVariant(
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
                   ),

                   // ─── Reload ─────────────────────
                   new CommandVariant(
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
                   ),

                   // ─── List ─────────────────────
                   new CommandVariant(
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
                   ),

                   // ─── Info ─────────────────────
                   new CommandVariant(
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
                   )
               }
           )
        {
        }
    }
}


namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public enum TokenKind
    {
        Unknown,
        Command,            // git / plugin
        Variant,            // commit / push / pull | load / unload / reload
        Option,             // --message
        OptionValue,        // value для option
        Flag,               // -a
        PositionalArgument,
        FlagValue
    }
}

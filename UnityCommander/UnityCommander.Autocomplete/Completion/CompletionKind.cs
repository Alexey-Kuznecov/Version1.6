namespace UnityCommander.Autocomplete.Completion
{
    public enum CompletionKind
    {
        None,
        Command,
        PositionalArgument,
        Flag,
        FlagValue,
        Nothing,
        Error,
        Variant
    }
}

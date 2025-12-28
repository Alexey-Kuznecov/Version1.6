namespace UnityCommander.Abstractions.Completion
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
        Variant,
        Container
    }
}

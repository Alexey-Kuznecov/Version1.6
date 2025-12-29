namespace UnityCommander.Abstractions.Completion
{
    public enum CompletionKind
    {
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

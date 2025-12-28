namespace UnityCommander.Abstractions.Completion
{
    public enum FlagOrderPolicy
    {
        AnyOrder,      // по умолчанию
        AfterPositionalArguments,
        StrictOrder    // если вдруг нужно
    }
}

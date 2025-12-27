namespace UnityCommander.Autocomplete.Infrastructure
{
    public enum FlagOrderPolicy
    {
        AnyOrder,      // по умолчанию
        AfterPositionalArguments,
        StrictOrder    // если вдруг нужно
    }
}

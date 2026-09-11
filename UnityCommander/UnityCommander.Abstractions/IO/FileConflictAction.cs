
namespace UnityCommander.Abstractions.IO
{
    public enum FileConflictAction
    {
        Replace,
        ReplaceAll,
        Skip,
        SkipAll,
        KeepBoth,
        Cancel,
        Rename
    }
}

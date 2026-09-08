
namespace UnityCommander.UI.Interaction
{
    public sealed record FileContext(
      string Path) : IInteractionContext;

    public sealed record DirectoryContext(
        string Path) : IInteractionContext;

    public sealed record DiskContext(
        string Path) : IInteractionContext;
}

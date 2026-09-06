
namespace UnityCommander.Index.Models
{
    public sealed record IndexChange(
     string Path,
     IndexChangeType Type,
     string? OldPath = null);
}

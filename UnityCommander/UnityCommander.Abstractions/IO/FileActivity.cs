
namespace UnityCommander.Abstractions.IO
{
    public sealed record FileActivity(
     Guid ItemId,
     string SourcePath,
     string DestinationPath);
}

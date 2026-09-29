
namespace UnityCommander.Abstractions.Thumbnail
{
    public sealed record ThumbnailResult(
         byte[] PngData,
         int Width,
         int Height);
}

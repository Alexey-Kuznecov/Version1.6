
namespace UnityCommander.Abstractions.Thumbnail
{
    public readonly record struct ThumbnailRequest(int Size)
    {
        public int Size { get; } = Size > 0
            ? Size
            : throw new ArgumentOutOfRangeException(nameof(Size));
    }
}

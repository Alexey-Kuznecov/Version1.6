
namespace UnityCommander.Abstractions.IO
{
    public sealed class FileConflict
    {
        public required string SourcePath { get; init; }
        public required string DestinationPath { get; init; }

        public bool SourceIsDirectory { get; init; }
        public bool DestinationIsDirectory { get; init; }
    }
}

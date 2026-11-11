
namespace UnityCommander.Diagnostics.Storage;

public sealed class DiagnosticsStorageOptions
{
    public int QueueCapacity { get; set; } = 8192;

    public int BatchSize { get; set; } = 256;

    public TimeSpan FlushInterval { get; set; } =
        TimeSpan.FromMilliseconds(500);

    public void Validate()
    {
        if (QueueCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(QueueCapacity));
        }

        if (BatchSize <= 0 || BatchSize > QueueCapacity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(BatchSize));
        }

        if (FlushInterval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(FlushInterval));
        }
    }
}
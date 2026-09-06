
namespace UnityCommander.Abstractions.IO
{
    public class CopyProgress
    {
        public double Percentage;

        public long Speed;

        public TimeSpan RemainingTime;

        public string? CurrentFile;

        public long BytesCopied;
    }
}

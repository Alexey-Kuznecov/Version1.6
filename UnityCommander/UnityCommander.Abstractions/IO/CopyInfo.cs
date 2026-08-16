
namespace UnityCommander.Abstractions.IO
{
    public class CopyInfo
    {
        public Guid OperationId { get; set; }
        public Guid ItemId { get; set; }

        public string Name { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Root { get; set; }

        public FileInfo FileInfo { get; set; }

        public long Length { get; set; }

        public double AverageSpeed { get; set; }

        public TimeSpan TimeLeft { get; set; }

        public TimeSpan TotalTimeLeft { get; set; }

        public double Percentage { get; set; }

        public double TotalPercentage { get; set; }

        public double ByteDone { get; set; }

        public double TotalBytes { get; set; }

        public double TotalByteDone { get; set; }

        public double CurrentFileSize { get; set; }

        public double TotalFileSize { get; set; }

        public bool Skipped { get; set; }

        public CopyDialogSkipReplaceStatus DialogSkipReplaceStatus { get; set; }

#region Log Properties

        public long CurrentBytesTransferred { get; set; }

        public long TotalBytesTransferred { get; set; }

#endregion
    }
}

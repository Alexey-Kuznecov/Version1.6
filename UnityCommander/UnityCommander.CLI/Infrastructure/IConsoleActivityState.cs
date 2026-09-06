namespace UnityCommander.CLI.Infrastructure
{
    public interface IConsoleActivityState
    {
        public string Title { get; set; }

        public string Status { get; set; }

        public long Found { get; set; }

        public long Processed { get; set; }

        public long Skipped { get; set; }

        public long? Total { get; set; }

        public TimeSpan Elapsed { get; set; }

        public double? Progress { get; }
    }
}
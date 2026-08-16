
using System;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Operation
{
    public class CopyProgressCalculator : ICopyProgressCalculator
    {
        public ProgressModel Calculate(OperationState state)
        {
            return new ProgressModel
            {
                Percent = (int)Math.Round(state.TotalBytes > 0 ? (double)state.CompletedBytes / state.TotalBytes * 100 : 0),
                ExactPercent = state.TotalBytes > 0 ? (double)state.CompletedBytes / state.TotalBytes * 100 : 0,
                Speed = FormatBytes(state.Speed),
                Remainder = $"{FormatBytes(state.CompletedBytes)} of {FormatBytes(state.TotalBytes)}",
                TimeLeft = ConvertTimeLeft(TimeSpan.FromSeconds(state.TotalBytes > 0 ? (state.TotalBytes - state.CompletedBytes) / Math.Max(state.Speed, 1) : 0))
            };
        }

        private string FormatBytes(double bytes)
        {
            if (bytes > 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024 * 1024):F2} GB";
            if (bytes > 1024 * 1024) return $"{bytes / (1024 * 1024):F2} MB";
            if (bytes > 1024) return $"{bytes / 1024:F2} KB";
            return $"{bytes:F0} B";
        }

        private string ConvertTimeLeft(TimeSpan time)
        {
            if (time.TotalSeconds < 1) return "Calculating..";
            if (time.Hours > 0) return $"{time.Hours} h {time.Minutes} min {time.Seconds} sec";
            if (time.Minutes > 0) return $"{time.Minutes} min {time.Seconds} sec";
            return $"{time.Seconds} sec";
        }
    }
}

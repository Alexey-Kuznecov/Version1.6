using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class CopyProgressCalculator
    {
        public ProgressModel Calculate(CopyInfo info)
        {
            return new ProgressModel
            {
                Percent = (int)Math.Round(info.TotalPercentage),
                ExactPercent = info.TotalPercentage,
                Speed = FormatBytes(info.AverageSpeed),
                Remainder = $"{FormatBytes(info.TotalByteDone)} of {FormatBytes(info.TotalBytes)}",
                TimeLeft = ConvertTimeLeft(info.TotalTimeLeft)
            };
        }

        private string FormatBytes(double bytes)
        {
            // Простейшая конвертация в K/M/G
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

    public class ProgressModel
    {
        public int Percent { get; set; }
        public double ExactPercent { get; set; }
        public string Speed { get; set; }
        public string Remainder { get; set; }
        public string TimeLeft { get; set; }
    }
}

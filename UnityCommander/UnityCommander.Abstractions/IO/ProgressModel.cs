
namespace UnityCommander.Abstractions.IO
{
    public class ProgressModel
    {
        public int Percent { get; set; }
        public double ExactPercent { get; set; }
        public string Speed { get; set; }
        public string Remainder { get; set; }
        public string TimeLeft { get; set; }
    }
}

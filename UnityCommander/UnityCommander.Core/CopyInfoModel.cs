using System;

namespace UnityCommander.Core
{
    public class CopyInfoModel
    {
        public string TargetFile { get; set; }
        public string SourceFile { get; set; }
        public int TotalLeft { get; set; }
        public DateTime AmazingTime { get; set; }
        public long FileLength { get; set; }
        public int CopyStatus { get; set; }
    }
}

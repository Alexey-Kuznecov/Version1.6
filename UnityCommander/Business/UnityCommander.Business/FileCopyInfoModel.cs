using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Business
{
    public class FileCopyInfoModel
    {
        public string TargetFile { get; set; }
        public string SourceFile { get; set; }
        public int TotalLeft { get; set; }
        public DateTime AmazingTime { get; set; }
    }
}

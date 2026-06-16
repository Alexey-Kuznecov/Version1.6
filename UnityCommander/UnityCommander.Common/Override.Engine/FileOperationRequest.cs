
using System.Collections.Generic;

namespace UnityCommander.Common.Override.Engine
{
    public class FileOperationRequest
    {
        public List<string> Sources { get; set; }
            = new List<string>();
        public string Target { get; set; }
        public bool ShowDialog { get; set; }
    }
}

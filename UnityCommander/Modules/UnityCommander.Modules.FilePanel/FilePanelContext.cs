
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel
{
    public class FilePanelContext
    {
        public string CurrentPath { get; set; } = default!;
        public List<string> SelectedFiles { get; set; } = new();
    }
}

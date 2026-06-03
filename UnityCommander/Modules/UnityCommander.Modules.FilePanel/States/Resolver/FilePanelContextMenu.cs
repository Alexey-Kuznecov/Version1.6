
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.States.Resolver
{
    public class FilePanelContextMenu
    {
        public string CurrentPath { get; set; } = default!;
        public List<string> SelectedFiles { get; set; } = new();
    }
}

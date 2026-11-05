
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.States.Resolver.Models
{
    public class FileContextMenu
    {
        public string CurrentPath { get; set; } = default!;
        
        public List<string> SelectedPaths { get; set; } = new();
    }
}

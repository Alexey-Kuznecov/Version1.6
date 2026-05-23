
using Prism.Commands;
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class MenuItemViewModel
    {
        public string Title { get; set; } = string.Empty;

        public DelegateCommand? Command { get; set; }

        public List<MenuItemViewModel> Children { get; set; } = new();

        // удобно для UI
        public bool HasChildren => Children.Count > 0;
    }
}

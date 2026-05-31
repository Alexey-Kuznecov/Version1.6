
using System;
using UnityCommander.Common.Models;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.States
{
    public class TabState
    {
        private string _currentPath;

        public event Action<string> CurrentPathChanged;

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                if (_currentPath == value)
                    return;

                _currentPath = value;
                CurrentPathChanged?.Invoke(value);
            }
        }

        public Guid TabId { get; set; }

        public PanelMode Mode { get; set; }

        public ContentRole Role { get; set; }

        public ViewMode ViewMode { get; set; }
    }
}

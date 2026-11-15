
namespace UnityCommander.Abstractions.Panels
{
    public class TabState
    {
        private string? _currentPath;

        public event Action<string>? CurrentPathChanged;

        public event Action<ViewMode>? ViewModeChanged;

        private ViewMode _viewMode;

        public ViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                if (_viewMode == value)
                    return;

                _viewMode = value;
                ViewModeChanged?.Invoke(value);
            }
        }

        public string? CurrentPath
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

        public Guid PanelId { get; set; }
    }
}

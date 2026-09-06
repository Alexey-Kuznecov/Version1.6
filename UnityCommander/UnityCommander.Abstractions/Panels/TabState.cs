
namespace UnityCommander.Abstractions.Panels
{
    public class TabState
    {
        private string? _currentPath;

        public event Action<string>? CurrentPathChanged;

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
    }
}

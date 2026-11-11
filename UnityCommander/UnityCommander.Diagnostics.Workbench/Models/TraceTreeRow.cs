
namespace UnityCommander.Diagnostics.Workbench.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class TraceTreeRow : INotifyPropertyChanged
    {
        private bool _isExpanded;

        private bool _isSearchVisible = true;
        private bool _isSearchMatch;
        private bool _isSearchForcedExpanded;

        private TraceEventItem? _searchMatchEvent;

        public TraceTreeRow(
            string title,
            string source,
            DateTime timestamp,
            double? durationMilliseconds,
            int indentLevel,
            TraceEventItem? representativeEvent,
            bool isFailed = false,
            IEnumerable<TraceEventItem>? searchEvents = null,
            string? nodeKey = null,
            bool isEvent = false)
        {
            Title = title;
            Source = source;
            Timestamp = timestamp;
            DurationMilliseconds = durationMilliseconds;
            IndentLevel = indentLevel;
            RepresentativeEvent = representativeEvent;
            IsFailed = isFailed;

            SearchEvents = searchEvents is not null
               ? searchEvents.ToList()
               : representativeEvent is not null
                   ? new List<TraceEventItem> { representativeEvent }
                   : new List<TraceEventItem>();

            NodeKey = nodeKey;
            IsEvent = isEvent;
        }

        public string Title { get; }

        public string Source { get; }

        public DateTime Timestamp { get; }

        public double? DurationMilliseconds { get; }

        public int IndentLevel { get; }

        public TraceEventItem? RepresentativeEvent { get; }

        public bool IsFailed { get; }

        public bool IsEvent { get; }

        public string? NodeKey { get; }

        public IReadOnlyList<TraceEventItem> SearchEvents { get; }

        public List<TraceTreeRow> Children { get; } = new();

        public bool IsExpandable => Children.Count > 0;

        public bool CanToggleExpansion =>
            IsExpandable && !_isSearchForcedExpanded;

        public string IndentPrefix => new(' ', IndentLevel * 4);

        public string ExpandGlyph =>
            !IsExpandable ? string.Empty : IsExpanded ? "▼" : "▶";

        public bool IsSearchVisible => _isSearchVisible;

        public bool IsSearchMatch => _isSearchMatch;

        public bool IsContextOnly =>
            _isSearchVisible &&
            !_isSearchMatch &&
            _isSearchForcedExpanded;

        public bool IsSearchForcedExpanded => _isSearchForcedExpanded;

        public TraceEventItem? SearchMatchEvent => _searchMatchEvent;

        public TraceEventItem? EventForSelection =>
            IsEvent
                ? RepresentativeEvent
                : SearchMatchEvent ?? RepresentativeEvent;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                {
                    return;
                }

                _isExpanded = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ExpandGlyph));

                ExpandedChanged?.Invoke(this);
            }
        }

        internal void SetSearchState(
            bool isVisible,
            bool isMatch,
            TraceEventItem? matchedEvent,
            bool forceExpanded)
        {
            _isSearchVisible = isVisible;
            _isSearchMatch = isMatch;
            _searchMatchEvent = matchedEvent;
            _isSearchForcedExpanded = forceExpanded;

            OnPropertyChanged(nameof(IsSearchVisible));
            OnPropertyChanged(nameof(IsSearchMatch));
            OnPropertyChanged(nameof(IsContextOnly));
            OnPropertyChanged(nameof(SearchMatchEvent));
            OnPropertyChanged(nameof(EventForSelection));
            OnPropertyChanged(nameof(IsSearchForcedExpanded));
            OnPropertyChanged(nameof(CanToggleExpansion));
        }

        public void ResetSearchState()
        {
            SetSearchState(
                isVisible: true,
                isMatch: false,
                matchedEvent: null,
                forceExpanded: false);
        }

        public event Action<TraceTreeRow>? ExpandedChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}

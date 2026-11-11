
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Workbench.Models;
using UnityCommander.Diagnostics.Workbench.Services;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Tracing;

public sealed class TracingViewModel : ObservableObject
{
    private readonly IDiagnosticsReader _reader;

    private CancellationTokenSource? _loadCancellation;

    private string _searchText = string.Empty;

    private string? _loadError;

    private TraceEventItem? _selectedEvent;

    private Dictionary<string, bool>? _treeExpansionSnapshot;

    private bool _isCallTreeSearchActive;

    private bool _isUpdatingTreeExpansion;

    public TracingViewModel(IDiagnosticsReader reader)
    {
        _reader = reader;

        Events = new ObservableCollection<TraceEventItem>();

        FilteredEvents = CollectionViewSource.GetDefaultView(Events);
        FilteredEvents.Filter = FilterEvent;
    }

    public ObservableCollection<TraceEventItem> Events { get; }

    public ICollectionView FilteredEvents { get; }

    public ObservableCollection<TraceTreeRow> VisibleRows { get; } = new();

    private readonly List<TraceTreeRow> _rootRows = new();

    private TraceTreeRow? _selectedTreeRow;

    private bool _isCallTreeView;

    public bool IsCallTreeView
    {
        get => _isCallTreeView;
        set
        {
            if (SetProperty(ref _isCallTreeView, value))
            {
                OnPropertyChanged(nameof(IsTimelineView));
                OnPropertyChanged(nameof(SearchScopeLabel));
                OnPropertyChanged(nameof(SearchToolTip));

                ApplySearchToActiveView();
            }
        }
    }

    public string SearchScopeLabel =>
        IsCallTreeView ? "CALL TREE" : "TIMELINE";

    public string SearchToolTip =>
        IsCallTreeView
            ? "Search operations, nested events and event data"
            : "Search timeline events, sources, identifiers and event data";

    public bool IsTimelineView => !IsCallTreeView;

    public TraceTreeRow? SelectedTreeRow
    {
        get => _selectedTreeRow;
        set
        {
            if (SetProperty(ref _selectedTreeRow, value))
            {
                SelectedEvent = value?.EventForSelection;
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplySearchToActiveView();
            }
        }
    }

    public string? LoadError
    {
        get => _loadError;
        private set => SetProperty(ref _loadError, value);
    }

    public TraceEventItem? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            if (SetProperty(ref _selectedEvent, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ICommand CopyAllDetailsCommand =>
        new RelayCommand(CopyAllDetails);

    public ICommand CopyEventDataCommand =>
        new RelayCommand(CopyEventData);

    public string EventCount => Events.Count.ToString("N0");

    public string TraceCount => Events
        .Select(item => item.TraceId)
        .Distinct()
        .Count()
        .ToString("N0");

    public string SourceCount => Events
        .Select(item => item.Source)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Count()
        .ToString("N0");

    public Task LoadSessionAsync(DiagnosticSession? session)
    {
        _loadCancellation?.Cancel();
        _loadCancellation = null;

        ClearSessionData();

        LoadError = null;

        if (session is null)
        {
            return Task.CompletedTask;
        }

        var cancellation = new CancellationTokenSource();

        _loadCancellation = cancellation;

        return ReadSessionAsync(session, cancellation);
    }

    private async Task ReadSessionAsync(
        DiagnosticSession session,
        CancellationTokenSource cancellation)
    {
        try
        {
            var query = new DiagnosticRecordQuery(
                session.Id,
                Kind: DiagnosticRecordKind.Trace,
                Limit: 10000);

            var records = await _reader.ReadRecordsAsync(
                query,
                cancellation.Token);

            if (cancellation.IsCancellationRequested ||
                !ReferenceEquals(_loadCancellation, cancellation))
            {
                return;
            }

            var events = records
                .OrderBy(record => record.Timestamp)
                .Select(TraceEventItemMapper.Map)
                .ToList();

            Events.Clear();

            foreach (TraceEventItem traceEvent in events)
            {
                Events.Add(traceEvent);
            }

            UpdateSummary();

            BuildCallTree();

            ApplySearchToActiveView();
        }
        catch (OperationCanceledException)
            when (cancellation.IsCancellationRequested)
        {
            // Загрузка предыдущей сессии отменена.
        }
        catch (Exception exception)
        {
            if (ReferenceEquals(_loadCancellation, cancellation))
            {
                LoadError = exception.Message;
            }
        }
        finally
        {
            if (ReferenceEquals(_loadCancellation, cancellation))
            {
                _loadCancellation = null;
            }

            cancellation.Dispose();
        }
    }

    private void ClearSessionData()
    {
        Events.Clear();

        SelectedEvent = null;

        _treeExpansionSnapshot = null;
        _isCallTreeSearchActive = false;
        _isUpdatingTreeExpansion = false;

        UpdateSummary();

        VisibleRows.Clear();
        _rootRows.Clear();

        SelectedTreeRow = null;
    }

    private void ApplySearchToActiveView()
    {
        FilteredEvents.Refresh();

        if (IsTimelineView)
        {
            RestoreCallTreeSearchState();

            SelectedTreeRow = null;

            SelectedEvent = Events.FirstOrDefault(
                item => MatchesEvent(item, SearchText));

            return;
        }

        ApplyCallTreeSearch();
    }

    private void UpdateSummary()
    {
        OnPropertyChanged(nameof(EventCount));
        OnPropertyChanged(nameof(TraceCount));
        OnPropertyChanged(nameof(SourceCount));
    }

    private bool FilterEvent(object item)
    {
        return item is TraceEventItem traceEvent
            && MatchesEvent(traceEvent, SearchText);
    }

    private static bool MatchesEvent(
        TraceEventItem traceEvent,
        string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return true;
        }

        string query = searchText.Trim();

        return Contains(traceEvent.EventName)
            || Contains(traceEvent.Source)
            || Contains(traceEvent.TraceId)
            || Contains(traceEvent.SpanId)
            || Contains(traceEvent.ParentSpanId)
            || Contains(traceEvent.PluginId)
            || Contains(traceEvent.Data);

        bool Contains(string? value)
        {
            return value?.Contains(
                query,
                StringComparison.OrdinalIgnoreCase) == true;
        }
    }

    private void ApplyCallTreeSearch()
    {
        string query = SearchText.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            RestoreCallTreeSearchState();

            RebuildVisibleRows();

            SelectedTreeRow = null;
            SelectedEvent = Events.FirstOrDefault();

            return;
        }

        // Сохраняем пользовательское раскрытие только при первом
        // переходе от обычного дерева к поисковому режиму.
        if (!_isCallTreeSearchActive)
        {
            _treeExpansionSnapshot = CaptureTreeExpansionStates();

            _isCallTreeSearchActive = true;
        }

        _isUpdatingTreeExpansion = true;

        try
        {
            foreach (TraceTreeRow root in _rootRows)
            {
                ApplyTreeSearch(root, query);
            }
        }
        finally
        {
            _isUpdatingTreeExpansion = false;
        }

        RebuildVisibleRows();

        // Выбираем именно первое найденное событие,
        // а не родительскую операцию.
        TraceTreeRow? firstMatch = FindFirstMatchingEventRow(_rootRows);

        if (firstMatch is null)
        {
            SelectedTreeRow = null;
            SelectedEvent = null;
        }
        else
        {
            SelectedTreeRow = firstMatch;
        }
    }

    private Dictionary<string, bool> CaptureTreeExpansionStates()
    {
        var result = new Dictionary<string, bool>(
            StringComparer.Ordinal);

        foreach (TraceTreeRow root in _rootRows)
        {
            CaptureTreeExpansionState(root, result);
        }

        return result;
    }

    private static void CaptureTreeExpansionState(
        TraceTreeRow row,
        Dictionary<string, bool> result)
    {
        if (row.NodeKey is not null)
        {
            result[row.NodeKey] = row.IsExpanded;
        }

        foreach (TraceTreeRow child in row.Children)
        {
            CaptureTreeExpansionState(child, result);
        }
    }

    private void RestoreCallTreeSearchState()
    {
        if (!_isCallTreeSearchActive)
        {
            return;
        }

        _isUpdatingTreeExpansion = true;

        try
        {
            foreach (TraceTreeRow root in _rootRows)
            {
                RestoreTreeExpansionState(root);
                ResetTreeSearchState(root);
            }
        }
        finally
        {
            _isUpdatingTreeExpansion = false;
        }

        _treeExpansionSnapshot = null;
        _isCallTreeSearchActive = false;
    }

    private void RestoreTreeExpansionState(TraceTreeRow row)
    {
        if (row.NodeKey is not null &&
            _treeExpansionSnapshot is not null &&
            _treeExpansionSnapshot.TryGetValue(
                row.NodeKey,
                out bool isExpanded))
        {
            row.IsExpanded = isExpanded;
        }

        foreach (TraceTreeRow child in row.Children)
        {
            RestoreTreeExpansionState(child);
        }
    }

    private static void ResetTreeSearchState(TraceTreeRow row)
    {
        row.ResetSearchState();

        foreach (TraceTreeRow child in row.Children)
        {
            ResetTreeSearchState(child);
        }
    }

    private static TraceTreeRow? FindFirstMatchingEventRow(
    IEnumerable<TraceTreeRow> rows)
    {
        foreach (TraceTreeRow row in rows)
        {
            if (row.IsEvent && row.IsSearchMatch)
            {
                return row;
            }

            TraceTreeRow? childMatch = FindFirstMatchingEventRow(row.Children);

            if (childMatch is not null)
            {
                return childMatch;
            }
        }

        return null;
    }

    private bool ApplyTreeSearch(
        TraceTreeRow row,
        string query)
    {
        // Строка представляет отдельное событие.
        if (row.IsEvent)
        {
            TraceEventItem? traceEvent = row.RepresentativeEvent;

            bool isMatch = traceEvent is not null
                && MatchesEvent(traceEvent, query);

            row.SetSearchState(
                isVisible: isMatch,
                isMatch: isMatch,
                matchedEvent: isMatch ? traceEvent : null,
                forceExpanded: false);

            return isMatch;
        }

        // Ищем совпадение среди всех событий данной Span-группы.
        TraceEventItem? matchedEvent = row.SearchEvents.FirstOrDefault(
            item => MatchesEvent(item, query));

        bool hasVisibleChildren = false;

        // Проверяем все дочерние события и вложенные операции.
        foreach (TraceTreeRow child in row.Children)
        {
            if (ApplyTreeSearch(child, query))
            {
                hasVisibleChildren = true;
            }
        }

        bool isVisible =
            matchedEvent is not null ||
            hasVisibleChildren;

        row.SetSearchState(
            isVisible: isVisible,
            isMatch: matchedEvent is not null,
            matchedEvent: matchedEvent,
            forceExpanded: hasVisibleChildren);

        // Автоматически раскрываем только те операции,
        // через которые проходит путь к найденному событию.
        row.IsExpanded = hasVisibleChildren;

        return isVisible;
    }

    private void BuildCallTree()
    {
        VisibleRows.Clear();
        _rootRows.Clear();

        _treeExpansionSnapshot = null;
        _isCallTreeSearchActive = false;

        var rows = TraceTreeBuilder.Build(Events);

        int spanCount = Events.Count(item =>
            !string.IsNullOrWhiteSpace(item.SpanId));

        int rootSpanCount = Events
            .Where(item => !string.IsNullOrWhiteSpace(item.SpanId))
            .Select(item => new
            {
                item.TraceId,
                item.SpanId,
                item.ParentSpanId
            })
            .Where(item => string.IsNullOrWhiteSpace(item.ParentSpanId))
            .Select(item => (item.TraceId, item.SpanId))
            .Distinct()
            .Count();

        System.Diagnostics.Debug.WriteLine(
            $"[CallTree] Events: {Events.Count}, " +
            $"Spans: {spanCount}, " +
            $"Root spans: {rootSpanCount}, " +
            $"Tree rows: {rows.Count}");

        _rootRows.AddRange(rows);

        foreach (TraceTreeRow row in _rootRows)
        {
            SubscribeToExpansion(row);
        }
    }

    private void SubscribeToExpansion(TraceTreeRow row)
    {
        row.ExpandedChanged += OnTreeRowExpanded;

        foreach (TraceTreeRow child in row.Children)
        {
            SubscribeToExpansion(child);
        }
    }

    private void OnTreeRowExpanded(TraceTreeRow row)
    {
        if (_isUpdatingTreeExpansion)
        {
            return;
        }

        RebuildVisibleRows();
    }

    private void RebuildVisibleRows()
    {
        VisibleRows.Clear();

        foreach (TraceTreeRow root in _rootRows)
        {
            AddVisibleRow(root);
        }
    }

    private void AddVisibleRow(TraceTreeRow row)
    {
        if (_isCallTreeSearchActive && !row.IsSearchVisible)
        {
            return;
        }

        VisibleRows.Add(row);

        if (!row.IsExpanded)
        {
            return;
        }

        foreach (TraceTreeRow child in row.Children)
        {
            AddVisibleRow(child);
        }
    }

    private void CopyAllDetails()
    {
        if (SelectedEvent is not TraceEventItem traceEvent)
            return;

        Clipboard.SetText(BuildEventDetails(traceEvent));
    }

    private void CopyEventData()
    {
        if (SelectedEvent is not TraceEventItem traceEvent)
            return;

        Clipboard.SetText(traceEvent.Data ?? string.Empty);
    }

    private static string BuildEventDetails(TraceEventItem item)
    {
        var builder = new StringBuilder();

        builder.AppendLine("EVENT DETAILS");
        builder.AppendLine("=============");
        builder.AppendLine();

        builder.AppendLine($"Event: {item.EventName}");
        builder.AppendLine($"Source: {item.Source}");
        builder.AppendLine(
            $"Timestamp: {item.Timestamp.ToString(
                "yyyy-MM-dd HH:mm:ss.fff",
                CultureInfo.InvariantCulture)}");

        builder.AppendLine(
            $"Duration: {FormatDuration(item.DurationMilliseconds)}");

        builder.AppendLine($"Trace ID: {FormatValue(item.TraceId)}");
        builder.AppendLine($"Span ID: {FormatValue(item.SpanId)}");
        builder.AppendLine($"Parent Span ID: {FormatValue(item.ParentSpanId)}");
        builder.AppendLine($"Nesting Depth: {item.Depth}");
        builder.AppendLine($"Plugin ID: {FormatValue(item.PluginId)}");

        builder.AppendLine();
        builder.AppendLine("EVENT DATA");
        builder.AppendLine("==========");
        builder.AppendLine();

        builder.AppendLine(item.Data ?? string.Empty);

        return builder.ToString();
    }

    private static string FormatValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "<none>"
            : value;
    }

    private static string FormatDuration(double? milliseconds)
    {
        return milliseconds.HasValue
            ? $"{milliseconds.Value.ToString("F2", CultureInfo.InvariantCulture)} ms"
            : "<none>";
    }
}


using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Workbench.Models;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Performance;

public sealed class PerformanceViewModel : ObservableObject
{
    private readonly IDiagnosticsReader _reader;
    private string _searchText = string.Empty;
    private string _selectedScope = "All";
    private PerformanceOperationItem? _selectedOperation;

    private CancellationTokenSource? _loadCancellation;

    private string? _loadError;

    private double[] _durationMilliseconds = Array.Empty<double>();

    private readonly List<DiagnosticRecordEntry> _records = new();

    public PerformanceViewModel(IDiagnosticsReader reader)
    {
        _reader = reader;

        Operations = new ObservableCollection<PerformanceOperationItem>();
        Scopes = new ObservableCollection<string>
        {
            "All",
            "Core",
            "Plugins"
        };

        SlowMeasurements = new ObservableCollection<PerformanceMeasurementItem>();

        FilteredOperations = CollectionViewSource.GetDefaultView(Operations);
        FilteredOperations.Filter = FilterOperation;
    }

    public ObservableCollection<PerformanceOperationItem> Operations { get; }

    public ICollectionView FilteredOperations { get; }

    public ObservableCollection<string> Scopes { get; }

    public ObservableCollection<PerformanceMeasurementItem> SlowMeasurements { get; }

    public string OperationCount => Operations.Count.ToString("N0");

    public string MeasurementCount => _records.Count.ToString("N0");

    public string AverageDuration =>
        _durationMilliseconds.Length == 0
            ? "—"
            : $"{_durationMilliseconds.Average():N1} ms";

    public string P95Duration
    {
        get
        {
            if (_durationMilliseconds.Length == 0)
            {
                return "—";
            }

            int index = (int)Math.Ceiling(
                _durationMilliseconds.Length * 0.95) - 1;

            return $"{_durationMilliseconds[index]:N1} ms";
        }
    }

    public string? LoadError
    {
        get => _loadError;
        private set => SetProperty(ref _loadError, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilteredOperations.Refresh();
            }
        }
    }

    public string SelectedScope
    {
        get => _selectedScope;
        set
        {
            if (SetProperty(ref _selectedScope, value))
            {
                FilteredOperations.Refresh();
            }
        }
    }

    public PerformanceOperationItem? SelectedOperation
    {
        get => _selectedOperation;
        set
        {
            if (SetProperty(ref _selectedOperation, value))
            {
                RefreshMeasurements();
            }
        }
    }

    private void RefreshMeasurements()
    {
        SlowMeasurements.Clear();

        if (SelectedOperation == null)
        {
            return;
        }

        var records = _records
          .Where(x =>
              x.Name == SelectedOperation.Operation &&
              GetOwner(x.PluginId) == SelectedOperation.Owner &&
              x.DurationTicks.HasValue)
          .OrderByDescending(x => x.DurationTicks)
          .Take(100);

        foreach (var record in records)
        {
            double milliseconds =
                record.DurationTicks.GetValueOrDefault()
                / (double)TimeSpan.TicksPerMillisecond;

            SlowMeasurements.Add(
                new PerformanceMeasurementItem(
                    record.Timestamp.LocalDateTime,
                    milliseconds,
                    record.PayloadJson));
        }
    }

    public Task LoadSessionAsync(DiagnosticSession? session)
    {
        _loadCancellation?.Cancel();
        _loadCancellation = null;

        ClearSessionData();
        LoadError = null;

        if (session == null)
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
                Kind: DiagnosticRecordKind.Measurement,
                Source: "Performance",
                Limit: 10000);

            var records = await _reader.ReadRecordsAsync(
                query,
                cancellation.Token);

            if (cancellation.IsCancellationRequested ||
                !ReferenceEquals(_loadCancellation, cancellation))
            {
                return;
            }

            _records.Clear();
            _records.AddRange(records);

            UpdateSummary();
            BuildOperations();
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

    private void BuildOperations()
    {
        Operations.Clear();

        var groups = _records
            .Where(x => x.DurationTicks.HasValue)
            .GroupBy(x => new
            {
                x.Name,
                Owner = GetOwner(x.PluginId)
            })
            .Select(group =>
            {
                var statistics = group
                    .Select(x => TimeSpan.FromTicks(x.DurationTicks!.Value))
                    .OrderBy(x => x)
                    .ToArray();

                double total = statistics.Sum(
                    x => x.TotalMilliseconds);

                double average = total / statistics.Length;

                double min = statistics.First().TotalMilliseconds;
                double max = statistics.Last().TotalMilliseconds;

                int p95Index = (int)Math.Ceiling(
                    statistics.Length * 0.95) - 1;

                double p95 = statistics[p95Index].TotalMilliseconds;

                return new PerformanceOperationItem(
                    group.Key.Name,
                    "General",
                    group.Key.Owner,
                    statistics.Length,
                    total,
                    average,
                    p95,
                    min,
                    max);
            })
            .ToArray();

        foreach (var operation in groups)
        {
            Operations.Add(operation);
        }

        OnPropertyChanged(nameof(OperationCount));

        SelectedOperation = FilteredOperations
            .Cast<PerformanceOperationItem>()
            .FirstOrDefault();
    }

    private static string GetOwner(string? pluginId)
    {
        return string.IsNullOrWhiteSpace(pluginId)
            ? "Core"
            : pluginId;
    }

    private bool FilterOperation(object item)
    {
        if (item is not PerformanceOperationItem operation)
        {
            return false;
        }

        bool matchesScope = SelectedScope switch
        {
            "Core" => operation.Owner == "Core",
            "Plugins" => operation.Owner != "Core",
            _ => true
        };

        bool matchesSearch = string.IsNullOrWhiteSpace(SearchText)
            || operation.Operation.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || operation.Owner.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || operation.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

        return matchesScope && matchesSearch;
    }

    private void UpdateSummary()
    {
        _durationMilliseconds = _records
            .Where(x => x.DurationTicks.HasValue)
            .Select(x =>
                x.DurationTicks!.Value /
                (double)TimeSpan.TicksPerMillisecond)
            .OrderBy(x => x)
            .ToArray();

        OnPropertyChanged(nameof(MeasurementCount));
        OnPropertyChanged(nameof(AverageDuration));
        OnPropertyChanged(nameof(P95Duration));
    }

    private void ClearSessionData()
    {
        SelectedOperation = null;

        Operations.Clear();
        SlowMeasurements.Clear();

        _records.Clear();

        _durationMilliseconds = Array.Empty<double>();

        OnPropertyChanged(nameof(OperationCount));
        OnPropertyChanged(nameof(MeasurementCount));
        OnPropertyChanged(nameof(AverageDuration));
        OnPropertyChanged(nameof(P95Duration));
    }
}

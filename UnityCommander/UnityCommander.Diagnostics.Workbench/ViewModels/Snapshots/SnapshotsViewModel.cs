using System.Collections.ObjectModel;
using System.Text.Json;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Abstraction.Performance;
using UnityCommander.Diagnostics.Workbench.Models;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Snapshots;

public sealed class SnapshotsViewModel : ObservableObject
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IDiagnosticsReader _reader;

    private CancellationTokenSource? _loadCancellation;

    private SnapshotItem? _selectedSnapshot;
    private SnapshotItem? _comparisonBefore;
    private SnapshotItem? _comparisonAfter;

    private string _comparisonStatus = "Select two snapshots to compare.";
    private string? _loadError;

    public SnapshotsViewModel(IDiagnosticsReader reader)
    {
        _reader = reader;

        Snapshots = new ObservableCollection<SnapshotItem>();
        ComparisonRows = new ObservableCollection<SnapshotComparisonItem>();

        CompareCommand = new RelayCommand(
            CompareSnapshots,
            CanCompareSnapshots);
    }

    public ObservableCollection<SnapshotItem> Snapshots { get; }

    public ObservableCollection<SnapshotComparisonItem> ComparisonRows { get; }

    public RelayCommand CompareCommand { get; }

    public SnapshotItem? SelectedSnapshot
    {
        get => _selectedSnapshot;
        set => SetProperty(ref _selectedSnapshot, value);
    }

    public SnapshotItem? ComparisonBefore
    {
        get => _comparisonBefore;
        set
        {
            if (SetProperty(ref _comparisonBefore, value))
            {
                CompareCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public SnapshotItem? ComparisonAfter
    {
        get => _comparisonAfter;
        set
        {
            if (SetProperty(ref _comparisonAfter, value))
            {
                CompareCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ComparisonStatus
    {
        get => _comparisonStatus;
        private set => SetProperty(ref _comparisonStatus, value);
    }

    public string? LoadError
    {
        get => _loadError;
        private set => SetProperty(ref _loadError, value);
    }

    public string SnapshotCount => Snapshots.Count.ToString("N0");

    public Task LoadSessionAsync(DiagnosticSession? session)
    {
        _loadCancellation?.Cancel();
        _loadCancellation = null;

        ClearSessionData();

        if (session == null)
        {
            ComparisonStatus = "No session selected.";
            return Task.CompletedTask;
        }

        var cancellation = new CancellationTokenSource();

        _loadCancellation = cancellation;

        return ReadSnapshotsAsync(session, cancellation);
    }

    private async Task ReadSnapshotsAsync(
        DiagnosticSession session,
        CancellationTokenSource cancellation)
    {
        try
        {
            var query = new DiagnosticRecordQuery(
                session.Id,
                Kind: DiagnosticRecordKind.Snapshot,
                Source: "Performance",
                Limit: 10000);

            var records = await _reader.ReadRecordsAsync(
                query,
                cancellation.Token);

            if (!IsCurrentLoad(cancellation))
            {
                return;
            }

            int skippedRecords = 0;

            var snapshots = new List<SnapshotItem>();

            foreach (var record in records.OrderByDescending(x => x.Timestamp))
            {
                if (!IsCurrentLoad(cancellation))
                {
                    return;
                }

                try
                {
                    var snapshot = JsonSerializer.Deserialize<PerformanceSnapshot>(
                        record.PayloadJson,
                        JsonOptions);

                    if (snapshot == null)
                    {
                        skippedRecords++;
                        continue;
                    }

                    snapshots.Add(
                        new SnapshotItem(record.Id, snapshot));
                }
                catch (JsonException)
                {
                    skippedRecords++;
                }
                catch (NotSupportedException)
                {
                    skippedRecords++;
                }
            }

            if (!IsCurrentLoad(cancellation))
            {
                return;
            }

            foreach (var snapshot in snapshots)
            {
                Snapshots.Add(snapshot);
            }

            SelectedSnapshot = Snapshots.FirstOrDefault();

            OnPropertyChanged(nameof(SnapshotCount));

            if (skippedRecords > 0)
            {
                LoadError =
                    $"Could not read {skippedRecords} snapshot record(s).";
            }

            ComparisonStatus = Snapshots.Count == 0
                ? "No snapshots in this session."
                : "Select two snapshots to compare.";
        }
        catch (OperationCanceledException)
            when (cancellation.IsCancellationRequested)
        {
            // Предыдущая загрузка отменена.
        }
        catch (Exception exception)
        {
            if (IsCurrentLoad(cancellation))
            {
                LoadError = exception.Message;
                ComparisonStatus = "Failed to load snapshots.";
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

    private bool IsCurrentLoad(
        CancellationTokenSource cancellation)
    {
        return !cancellation.IsCancellationRequested
            && ReferenceEquals(_loadCancellation, cancellation);
    }

    private void ClearSessionData()
    {
        Snapshots.Clear();
        ComparisonRows.Clear();

        SelectedSnapshot = null;
        ComparisonBefore = null;
        ComparisonAfter = null;

        LoadError = null;

        ComparisonStatus = "Loading snapshots...";

        OnPropertyChanged(nameof(SnapshotCount));
    }

    private bool CanCompareSnapshots()
    {
        return ComparisonBefore != null
            && ComparisonAfter != null
            && ComparisonBefore.RecordId != ComparisonAfter.RecordId;
    }

    private void CompareSnapshots()
    {
        ComparisonRows.Clear();

        if (!CanCompareSnapshots())
        {
            ComparisonStatus = "Select two different snapshots to compare.";
            return;
        }

        var before = ComparisonBefore!.Data.Statistics;
        var after = ComparisonAfter!.Data.Statistics;

        var operations = before.Keys
            .Union(after.Keys, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal);

        foreach (var operation in operations)
        {
            bool hasBefore = before.TryGetValue(
                operation,
                out var beforeStats);

            bool hasAfter = after.TryGetValue(
                operation,
                out var afterStats);

            ComparisonRows.Add(
                new SnapshotComparisonItem(
                    operation,
                    hasBefore
                        ? beforeStats!.Average.TotalMilliseconds
                        : null,
                    hasAfter
                        ? afterStats!.Average.TotalMilliseconds
                        : null));
        }

        ComparisonStatus =
            $"Comparing {ComparisonBefore.Name} → {ComparisonAfter.Name}";

        if (ComparisonRows.Count == 0)
        {
            ComparisonStatus = "No operations available for comparison.";
        }
    }
}

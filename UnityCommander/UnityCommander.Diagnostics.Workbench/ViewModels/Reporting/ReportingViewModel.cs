using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Workbench.Models;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Reporting;


public sealed class ReportingViewModel : ObservableObject
{
    private readonly IDiagnosticsReader _reader;

    private readonly List<DiagnosticRecordEntry> _records = new();

    private CancellationTokenSource? _loadCancellation;

    private DiagnosticSourceItem? _selectedSource;
    private DiagnosticResultItem? _selectedResult;
    private string _diagnosticMode = "Report";
    private string? _loadError;

    private bool _isComponentFilterOpen;
    private bool _isUpdatingComponentSelection;

    public ReportingViewModel(IDiagnosticsReader reader)
    {
        _reader = reader;

        Sources = new ObservableCollection<DiagnosticSourceItem>();
        Results = new ObservableCollection<DiagnosticResultItem>();

        DiagnosticModes = new ObservableCollection<string>
        {
            "State",
            "Report"
        };

        ComponentFilters = new ObservableCollection<DiagnosticComponentFilterItem>();
    }

    public ObservableCollection<DiagnosticSourceItem> Sources { get; }

    public ObservableCollection<DiagnosticResultItem> Results { get; }

    public ObservableCollection<string> DiagnosticModes { get; }

    public DiagnosticSourceItem? SelectedSource
    {
        get => _selectedSource;
        set
        {
            if (SetProperty(ref _selectedSource, value))
            {
                RefreshResults();
            }
        }
    }

    public DiagnosticResultItem? SelectedResult
    {
        get => _selectedResult;
        set => SetProperty(ref _selectedResult, value);
    }

    public string DiagnosticMode
    {
        get => _diagnosticMode;
        set => SetProperty(ref _diagnosticMode, value);
    }

    public string? LoadError
    {
        get => _loadError;
        private set => SetProperty(ref _loadError, value);
    }

    public string SourceCount => Sources.Count.ToString("N0");

    public string ResultCount => _records.Count.ToString("N0");

    public string WarningCount => _records.Count(IsWarningRecord).ToString("N0");

    public ICommand CopyJsonCommand =>
    new RelayCommand(CopyJson);

    private void CopyJson()
    {
        if (SelectedResult == null)
            return;

        Clipboard.SetText(SelectedResult.Details ?? string.Empty);
    }

    #region GLOBAL FILTER

    public ObservableCollection<DiagnosticComponentFilterItem> ComponentFilters { get; }

    public bool IsComponentFilterOpen
    {
        get => _isComponentFilterOpen;
        set => SetProperty(ref _isComponentFilterOpen, value);
    }

    public bool AllComponentsSelected
    {
        get => ComponentFilters.Count > 0 &&
               ComponentFilters.All(x => x.IsSelected);

        set
        {
            _isUpdatingComponentSelection = true;

            try
            {
                foreach (var component in ComponentFilters)
                {
                    component.IsSelected = value;
                }
            }
            finally
            {
                _isUpdatingComponentSelection = false;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(ComponentFilterSummary));

            RefreshResults();
        }
    }

    public string ComponentFilterSummary
    {
        get
        {
            int selected = ComponentFilters.Count(x => x.IsSelected);

            return $"{selected} / {ComponentFilters.Count} components selected";
        }
    }

    private void BuildComponentFilters()
    {
        foreach (var component in ComponentFilters)
        {
            component.PropertyChanged -= OnComponentFilterItemPropertyChanged;
        }

        ComponentFilters.Clear();

        var names = _records
            .SelectMany(record => GetComponentNames(record.PayloadJson))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            var component = new DiagnosticComponentFilterItem(name);

            component.PropertyChanged += OnComponentFilterItemPropertyChanged;

            ComponentFilters.Add(component);
        }

        OnPropertyChanged(nameof(AllComponentsSelected));
        OnPropertyChanged(nameof(ComponentFilterSummary));
    }

    private void OnComponentFilterItemPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (_isUpdatingComponentSelection ||
            e.PropertyName != nameof(DiagnosticComponentFilterItem.IsSelected))
        {
            return;
        }

        OnPropertyChanged(nameof(AllComponentsSelected));
        OnPropertyChanged(nameof(ComponentFilterSummary));

        RefreshResults();
    }

    private static int GetFilteredComponentCount(
    JsonElement root,
    HashSet<string> selectedComponents)
    {
        if (!root.TryGetProperty("components", out var components) ||
            components.ValueKind != JsonValueKind.Array)
        {
            return 0;
        }

        int count = 0;

        foreach (var component in components.EnumerateArray())
        {
            if (!component.TryGetProperty("name", out var name) ||
                name.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            string? componentName = name.GetString();

            if (componentName != null &&
                selectedComponents.Contains(componentName))
            {
                count++;
            }
        }

        return count;
    }

    private static string FilterPayloadJson(
    JsonElement root,
    HashSet<string> selectedComponents)
    {
        using var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(
            stream,
            new JsonWriterOptions
            {
                Indented = true
            }))
        {
            writer.WriteStartObject();

            foreach (var property in root.EnumerateObject())
            {
                if (property.NameEquals("components") &&
                    property.Value.ValueKind == JsonValueKind.Array)
                {
                    writer.WritePropertyName(property.Name);
                    writer.WriteStartArray();

                    foreach (var component in property.Value.EnumerateArray())
                    {
                        if (!component.TryGetProperty("name", out var name) ||
                            name.ValueKind != JsonValueKind.String)
                        {
                            continue;
                        }

                        string? componentName = name.GetString();

                        if (componentName != null &&
                            selectedComponents.Contains(componentName))
                        {
                            component.WriteTo(writer);
                        }
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    #endregion

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
                Kind: DiagnosticRecordKind.Report,
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

            BuildComponentFilters();

            BuildSources();
            RefreshResults();

            OnPropertyChanged(nameof(ResultCount));
            OnPropertyChanged(nameof(WarningCount));
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

    private void BuildSources()
    {
        Sources.Clear();

        var sources = _records
            .Select(x => x.Source)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x);

        foreach (var source in sources)
        {
            Sources.Add(new DiagnosticSourceItem(
                source,
                $"Diagnostic events from {source}",
                "Source"));
        }

        OnPropertyChanged(nameof(SourceCount));
    }

    private void RefreshResults()
    {
        Results.Clear();

        var selectedComponents = ComponentFilters
            .Where(x => x.IsSelected)
            .Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var records = _records.AsEnumerable();

        if (SelectedSource != null)
        {
            records = records.Where(record =>
                string.Equals(
                    record.Source,
                    SelectedSource.Name,
                    StringComparison.OrdinalIgnoreCase));
        }

        foreach (var record in records)
        {
            Results.Add(CreateResult(record, selectedComponents));
        }

        SelectedResult = Results.FirstOrDefault();
    }

    private static DiagnosticResultItem CreateResult(
     DiagnosticRecordEntry record,
     HashSet<string> selectedComponents)
    {
        using var document = JsonDocument.Parse(record.PayloadJson);

        var root = document.RootElement;

        string status = GetStatus(root);

        if (HasComponentErrors(root))
        {
            status = "Warning";
        }
        else if (string.IsNullOrWhiteSpace(status))
        {
            status = "Info";
        }
        else if (IsError(status))
        {
            status = "Error";
        }

        int componentCount = GetFilteredComponentCount(
            root,
            selectedComponents);

        string summary =
            $"{record.Source} · {componentCount} components";

        if (status != "Info")
        {
            summary += $" · {status}";
        }

        string details = FilterPayloadJson(
            root,
            selectedComponents);

        return new DiagnosticResultItem(
            record.Name,
            status,
            summary,
            details);
    }

    private static List<string> GetComponentNames(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);

        var names = new List<string>();

        if (!document.RootElement.TryGetProperty(
                "components",
                out var components) ||
            components.ValueKind != JsonValueKind.Array)
        {
            return names;
        }

        foreach (var component in components.EnumerateArray())
        {
            if (!component.TryGetProperty("name", out var name) ||
                name.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            string? value = name.GetString();

            if (!string.IsNullOrWhiteSpace(value))
            {
                names.Add(value);
            }
        }

        return names;
    }

    private static string GetStatus(JsonElement root)
    {
        if (!root.TryGetProperty("eventPayload", out var payload) ||
            payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty("status", out var status) ||
            status.ValueKind != JsonValueKind.String)
        {
            return string.Empty;
        }

        return status.GetString() ?? string.Empty;
    }

    private static bool HasComponentErrors(JsonElement root)
    {
        if (!root.TryGetProperty("components", out var components) ||
            components.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var component in components.EnumerateArray())
        {
            if (!component.TryGetProperty("error", out var error))
            {
                continue;
            }

            if (error.ValueKind == JsonValueKind.Null ||
                error.ValueKind == JsonValueKind.Undefined)
            {
                continue;
            }

            if (error.ValueKind == JsonValueKind.String &&
                string.IsNullOrWhiteSpace(error.GetString()))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool IsError(string status)
    {
        return string.Equals(
                   status,
                   "Failed",
                   StringComparison.OrdinalIgnoreCase)
            || string.Equals(
                   status,
                   "Error",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsWarningRecord(DiagnosticRecordEntry record)
    {
        using var document = JsonDocument.Parse(record.PayloadJson);

        var root = document.RootElement;
        var status = GetStatus(root);

        return IsError(status)
            || string.Equals(status, "Warning", StringComparison.OrdinalIgnoreCase)
            || HasComponentErrors(root);
    }

    private void ClearSessionData()
    {
        _records.Clear();

        foreach (var component in ComponentFilters)
        {
            component.PropertyChanged -= OnComponentFilterItemPropertyChanged;
        }

        ComponentFilters.Clear();

        Sources.Clear();
        Results.Clear();

        SelectedSource = null;
        SelectedResult = null;

        IsComponentFilterOpen = false;

        OnPropertyChanged(nameof(SourceCount));
        OnPropertyChanged(nameof(ResultCount));
        OnPropertyChanged(nameof(WarningCount));
        OnPropertyChanged(nameof(AllComponentsSelected));
        OnPropertyChanged(nameof(ComponentFilterSummary));
    }
}

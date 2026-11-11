using System.Collections.ObjectModel;
using UnityCommander.Diagnostics.Workbench.Models;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Reporting;

public sealed class ReportingViewModel : ObservableObject
{
    private DiagnosticSourceItem? _selectedSource;
    private DiagnosticResultItem? _selectedResult;
    private string _diagnosticMode = "Report";

    public ReportingViewModel()
    {
        Sources = new ObservableCollection<DiagnosticSourceItem>
        {
            new("ApplicationState", "General application state and lifecycle", "Core"),
            new("FilePanelDiagnostics", "File panel initialization and content state", "File system"),
            new("PluginDiagnostics", "Loaded plugins and their reported state", "Plugins"),
            new("ThumbnailDiagnostics", "Thumbnail provider and cache state", "Preview")
        };

        DiagnosticModes = new ObservableCollection<string>
        {
            "State",
            "Report"
        };

        Results = new ObservableCollection<DiagnosticResultItem>
        {
            new("Application", "OK", "Application initialized", "Shell: Ready\nModules: 14 loaded\nActive workspace: Default"),
            new("FilePanel", "Warning", "Initial enumeration took longer than expected", "Path: D:\\Projects\\UnityCommander\nItems: 248\nElapsed: 124 ms"),
            new("ThumbnailService", "OK", "Thumbnail service is available", "Providers: 3\nCache: Enabled\nRequest size: 128 px")
        };

        SelectedSource = Sources[0];
        SelectedResult = Results[0];
    }

    public ObservableCollection<DiagnosticSourceItem> Sources { get; }

    public ObservableCollection<DiagnosticResultItem> Results { get; }

    public ObservableCollection<string> DiagnosticModes { get; }

    public DiagnosticSourceItem? SelectedSource
    {
        get => _selectedSource;
        set => SetProperty(ref _selectedSource, value);
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

    public string SourceCount => Sources.Count.ToString("N0");

    public string ResultCount => Results.Count.ToString("N0");

    public string WarningCount => Results.Count(item => item.Status == "Warning").ToString("N0");
}

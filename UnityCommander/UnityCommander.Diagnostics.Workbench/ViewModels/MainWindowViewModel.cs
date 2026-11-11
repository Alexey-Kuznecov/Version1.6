using System.Collections.ObjectModel;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;
using UnityCommander.Diagnostics.Workbench.ViewModels.Performance;
using UnityCommander.Diagnostics.Workbench.ViewModels.Reporting;
using UnityCommander.Diagnostics.Workbench.ViewModels.Snapshots;
using UnityCommander.Diagnostics.Workbench.ViewModels.Tracing;

namespace UnityCommander.Diagnostics.Workbench.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly IDiagnosticsReader _diagnosticsReader;

    private DiagnosticSession? _selectedSession;
    private Task _sessionChangeTask = Task.CompletedTask;

    private string? _sessionLoadError;

    public MainWindowViewModel(
        IDiagnosticsReader diagnosticsReader)
    {
        _diagnosticsReader = diagnosticsReader;

        Sessions = new ObservableCollection<DiagnosticSession>();

        Performance = new PerformanceViewModel(diagnosticsReader);

        Tracing = new TracingViewModel(diagnosticsReader);
        Reporting = new ReportingViewModel();
        Snapshots = new SnapshotsViewModel(diagnosticsReader);
    }

    public ObservableCollection<DiagnosticSession> Sessions { get; }

    public PerformanceViewModel Performance { get; }

    public TracingViewModel Tracing { get; }

    public ReportingViewModel Reporting { get; }

    public SnapshotsViewModel Snapshots { get; }

    public DiagnosticSession? SelectedSession
    {
        get => _selectedSession;
        set
        {
            if (SetProperty(ref _selectedSession, value))
            {
                SessionLoadError = null;
                _sessionChangeTask = ApplySelectedSessionAsync(value);
            }

            IsSessionPanelOpen = false;
        }
    }

    private bool _isSessionPanelOpen;

    public bool IsSessionPanelOpen
    {
        get => _isSessionPanelOpen;
        set => SetProperty(ref _isSessionPanelOpen, value);
    }

    public string? SessionLoadError
    {
        get => _sessionLoadError;
        private set => SetProperty(ref _sessionLoadError, value);
    }

    public string ApplicationTitle => "Unity Commander Diagnostics";

    public string ApplicationSubtitle => "Developer Workbench";

    public string ConnectionStatus => "Not connected";

    public string DataSourceDescription => "SQLite · Unity Commander Diagnostics";

    public async Task InitializeAsync()
    {
        var sessions = await _diagnosticsReader.GetSessionsAsync();

        Sessions.Clear();

        foreach (var session in sessions.OrderByDescending(x => x.StartedAt))
        {
            Sessions.Add(session);
        }

        SelectedSession = Sessions.FirstOrDefault();

        await _sessionChangeTask;
    }

    private async Task ApplySelectedSessionAsync(
        DiagnosticSession? session)
    {
        try
        {
            await Task.WhenAll(
               Performance.LoadSessionAsync(session),
               Snapshots.LoadSessionAsync(session),
               Tracing.LoadSessionAsync(session));

            // По мере реализации:
            // await Tracing.LoadSessionAsync(session);
            // await Reporting.LoadSessionAsync(session);
            // await Snapshots.LoadSessionAsync(session);
        }
        catch (Exception exception)
        {
            SessionLoadError = exception.Message;
        }
    }
}

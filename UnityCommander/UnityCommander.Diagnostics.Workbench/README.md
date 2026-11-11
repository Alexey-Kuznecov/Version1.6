# Unity Commander Diagnostics Workbench

Standalone WPF prototype for the Unity Commander diagnostics developer tool.

## Current scope

- Separate WPF executable (`net10.0-windows`).
- Four initial screens: Performance, Tracing, Reporting and Snapshots.
- Demonstration data only; no connection to Unity Commander or persistent storage yet.
- Performance and Tracing include search/filtering.
- Snapshots supports creating prototype entries and comparing two selected entries.

## Run

Open `UnityCommander.Diagnostics.Workbench.csproj` on Windows with the .NET 10 Windows Desktop SDK and run it.

## Integration notes

The prototype is intentionally self-contained so the UI can be reviewed before connecting it to the existing diagnostics infrastructure. The small `ObservableObject` and `RelayCommand` classes are local scaffolding for this standalone project; they can be replaced with the shared `UnityCommander.Mvvm.Base` types once the project is added to the solution and its exact project reference is known.

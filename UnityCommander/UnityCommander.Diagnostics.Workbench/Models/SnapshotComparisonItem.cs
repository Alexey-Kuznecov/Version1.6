namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed record SnapshotComparisonItem(
    string Operation,
    double? BeforeMilliseconds,
    double? AfterMilliseconds)
{
    public double? DifferenceMilliseconds =>
        BeforeMilliseconds.HasValue && AfterMilliseconds.HasValue
            ? AfterMilliseconds.Value - BeforeMilliseconds.Value
            : null;

    public double? DifferencePercent =>
        BeforeMilliseconds.HasValue
        && AfterMilliseconds.HasValue
        && BeforeMilliseconds.Value != 0
            ? (AfterMilliseconds.Value - BeforeMilliseconds.Value)
                / BeforeMilliseconds.Value * 100
            : null;

    public string Status =>
        BeforeMilliseconds == null
            ? "New"
            : AfterMilliseconds == null
                ? "Removed"
                : "Compared";
}
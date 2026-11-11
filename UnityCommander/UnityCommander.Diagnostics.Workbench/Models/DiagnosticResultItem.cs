namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed class DiagnosticResultItem
{
    public DiagnosticResultItem(string name, string status, string summary, string details)
    {
        Name = name;
        Status = status;
        Summary = summary;
        Details = details;
    }

    public string Name { get; }

    public string Status { get; }

    public string Summary { get; }

    public string Details { get; }
}

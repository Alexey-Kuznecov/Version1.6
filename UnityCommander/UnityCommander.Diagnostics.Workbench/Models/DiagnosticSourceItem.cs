namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed class DiagnosticSourceItem
{
    public DiagnosticSourceItem(string name, string description, string category)
    {
        Name = name;
        Description = description;
        Category = category;
    }

    public string Name { get; }

    public string Description { get; }

    public string Category { get; }
}

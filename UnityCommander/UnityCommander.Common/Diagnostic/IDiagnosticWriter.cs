
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticWriter
    {
        void Write(string value);
        void WriteLine(string value);

        void BeginSection(string title);
        void EndSection();

        void BeginTable(string title);
        void Row(string name, object? value);
        void EndTable();

        void Indent();

        void Inspect(object? value);
    }
}

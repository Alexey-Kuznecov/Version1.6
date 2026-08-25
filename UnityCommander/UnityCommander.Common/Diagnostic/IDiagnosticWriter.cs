
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticWriter
    {
        void Write(string value);
        void WriteLine(string value);

        //void BeginSection(string title);
        //void EndSection();

        //void Indent();

        ////void WriteTable(
        ////    IReadOnlyList<DiagnosticColumn> columns,
        ////    IReadOnlyList<IReadOnlyList<object?>> rows);

        //void Inspect(object? value);
    }
}

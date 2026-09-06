
using System.Collections.Generic;
using UnityCommander.CLI.Core;

namespace UnityCommander.Commands.Diagnostic
{
    internal sealed class DiagnosticTable
    {
        private readonly string _title;
        private readonly List<RowData> _rows = [];

        public DiagnosticTable(string title)
        {
            _title = title;
        }

        public void AddRow(string name, object? value)
        {
            _rows.Add(new RowData(
                name,
                value?.ToString() ?? "null"));
        }

        public void WriteTo(IConsoleOutput output)
        {
            output.WriteLine(_title);

            var width = GetNameWidth();

            output.WriteLine(new string('-', width + 4));
            output.WriteLine("");

            foreach (var row in _rows)
            {
                output.WriteLine(
                    $"{row.Name.PadRight(width + 4)}{row.Value}");
            }

            //output.WriteLine("");
            //output.WriteLine(new string('-', width + 4));
        }

        private int GetNameWidth()
        {
            var width = _title.Length;

            foreach (var row in _rows)
            {
                if (row.Name.Length > width)
                    width = row.Name.Length;
            }

            return width;
        }

        private readonly record struct RowData(
            string Name,
            string Value);
    }
}

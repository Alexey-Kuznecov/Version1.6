
using System;
using UnityCommander.CLI.Core;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Commands.Diagnostic
{
    internal sealed class DiagnosticConsoleWriter : IDiagnosticWriter
    {
        private readonly IConsoleOutput _output;

        private DiagnosticTable? _table;

        public DiagnosticConsoleWriter(IConsoleOutput output)
        {
            _output = output;
        }

        public void BeginTable(string title)
        {
            if (_table != null)
                throw new InvalidOperationException("A diagnostic table is already open.");

            _table = new DiagnosticTable(title);
        }

        public void Row(string name, object? value)
        {
            if (_table == null)
                throw new InvalidOperationException("No diagnostic table is open.");

            _table.AddRow(name, value);
        }

        public void EndTable()
        {
            if (_table == null)
                throw new InvalidOperationException("No diagnostic table is open.");

            var table = _table;
            _table = null;

            table.WriteTo(_output);
        }

        public void BeginSection(string title)
        {
            throw new NotImplementedException();
        }

        public void EndSection()
        {
            throw new NotImplementedException();
        }

        public void Indent()
        {
            throw new NotImplementedException();
        }

        public void Inspect(object? value)
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            _output.Write(value);
        }

        public void WriteLine(string value)
        {
            _output.WriteLine(value);
        }
    }
}

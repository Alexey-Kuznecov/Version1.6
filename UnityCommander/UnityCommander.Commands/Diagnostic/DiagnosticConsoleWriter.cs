
using System;
using UnityCommander.CLI.Core;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Commands.Diagnostic
{
    internal class DiagnosticConsoleWriter : IDiagnosticWriter
    {
        private readonly IConsoleOutput _output;

        public DiagnosticConsoleWriter(IConsoleOutput output)
        {
            _output = output;
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

        public void Inspect(object value)
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

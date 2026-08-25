
using System.Text;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Diagnostics.Diagnostic
{
    public sealed class DiagnosticBufferWriter : IDiagnosticWriter
    {
        private readonly StringBuilder _buffer = new();

        public void Write(string value)
            => _buffer.Append(value);

        public void WriteLine(string value)
            => _buffer.AppendLine(value);

        public override string ToString()
            => _buffer.ToString();
    }
}

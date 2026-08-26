
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

        public void BeginSection(string title)
        {
            throw new System.NotImplementedException();
        }

        public void EndSection()
        {
            throw new System.NotImplementedException();
        }

        public void BeginTable(string title)
        {
            throw new System.NotImplementedException();
        }

        public void Row(string name, object value)
        {
            throw new System.NotImplementedException();
        }

        public void EndTable()
        {
            throw new System.NotImplementedException();
        }

        public void Indent()
        {
            throw new System.NotImplementedException();
        }

        public void Inspect(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}

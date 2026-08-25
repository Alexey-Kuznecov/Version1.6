
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityCommander.CLI.Core;
using UnityCommander.Diagnostics.Diagnostic;

namespace UnityCommander.Commands.Diagnostic
{
    public sealed class DiagnosticRender : IDiagnosticRender
    {
        public void Render(
            IConsoleOutput output,
            DiagnosticResult result)
        {
            var visited = new HashSet<object>(
                ReferenceEqualityComparer.Instance);

            RenderValue(
                output,
                result.Values,
                visited,
                0);
        }

        private void RenderValue(
            IConsoleOutput output,
            object? value,
            HashSet<object> visited,
            int indent)
        {
            var padding = new string(' ', indent * 4);

            if (value is null)
            {
                output.WriteLine($"{padding}<null>");
                return;
            }

            if (value is string text)
            {
                output.WriteLine($"{padding}{text}");
                return;
            }

            if (!value.GetType().IsValueType)
            {
                if (!visited.Add(value))
                {
                    output.WriteLine($"{padding}<already rendered>");
                    return;
                }
            }

            if (value is IDictionary dictionary)
            {
                foreach (DictionaryEntry item in dictionary)
                {
                    output.WriteLine($"{padding}{item.Key}:");

                    RenderValue(
                        output,
                        item.Value,
                        visited,
                        indent + 1);
                }

                return;
            }

            if (value is IEnumerable collection)
            {
                var index = 0;

                foreach (var item in collection)
                {
                    output.WriteLine($"{padding}[{index}]");

                    RenderValue(
                        output,
                        item,
                        visited,
                        indent + 1);

                    index++;
                }

                return;
            }

            output.WriteLine($"{padding}{value}");
        }

        private sealed class ReferenceEqualityComparer
            : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();

            public bool Equals(
                object? x,
                object? y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(
                object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}


using System.Text;

namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceTableFormatter : IPerformanceTableFormatter
    {
        private const int MaxColumns = 6;
        private const int MaxColumnWidth = 36;

        public IEnumerable<string> Format(
            IReadOnlyList<PerformanceMeasurement> measurements)
        {
            if (measurements.Count == 0)
                yield break;

            var keys = measurements
                .SelectMany(x => x.Metadata.Keys)
                .Distinct(StringComparer.Ordinal)
                .Take(MaxColumns - 1)
                .ToArray();

            var columns = new[] { "Sequence", "Duration" }
                .Concat(keys)
                .ToArray();

            var rows = measurements
                .Select(x => CreateRow(x, keys))
                .ToArray();

            var widths = CalculateWidths(columns, rows);

            yield return BuildRow(columns, widths);
            yield return BuildSeparator(widths);

            foreach (var row in rows)
                yield return BuildRow(row, widths);
        }

        public IEnumerable<string> Format(
            IReadOnlyList<PerformanceItemStatistics> statistics)
        {
            if (statistics.Count == 0)
                yield break;

            var columns = new[]
            {
                "Items",
                "Samples",
                "Total",
                "Average",
                "P95",
                "Min",
                "Max"
            };

            var rows = statistics
                .Select(x => new[]
                {
                    x.Range.ToString(),
                    x.Samples.ToString(),
                    $"{x.Total.TotalMilliseconds:F2}",
                    $"{x.Average.TotalMilliseconds:F2}",
                    $"{x.P95.TotalMilliseconds:F2}",
                    $"{x.Min.TotalMilliseconds:F2}",
                    $"{x.Max.TotalMilliseconds:F2}"
                })
                .ToArray();

            var widths =
                CalculateWidths(columns, rows);

            yield return BuildRow(columns, widths);
            yield return BuildSeparator(widths);

            foreach (var row in rows)
                yield return BuildRow(row, widths);
        }

        private static string[] CreateRow(
            PerformanceMeasurement measurement,
            IReadOnlyList<string> keys)
        {
            var row = new string[keys.Count + 2];

            row[0] = measurement.Sequence.ToString();

            row[1] =
                measurement.Duration.TotalMilliseconds.ToString("F2");

            for (var i = 0; i < keys.Count; i++)
            {
                row[i + 2] =
                    measurement.Metadata.TryGetValue(
                        keys[i],
                        out var value)
                        ? FormatValue(value)
                        : string.Empty;
            }

            return row;
        }

        private static string FormatValue(object? value)
        {
            var text = value?.ToString() ?? "<null>";

            if (text.Length > MaxColumnWidth)
                return text[..(MaxColumnWidth - 1)] + "…";

            return text;
        }

        private static int[] CalculateWidths(
            IReadOnlyList<string> headers,
            IReadOnlyList<string[]> rows)
        {
            var widths = headers
                .Select(x => Math.Min(x.Length, MaxColumnWidth))
                .ToArray();

            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    widths[i] = Math.Min(
                        MaxColumnWidth,
                        Math.Max(
                            widths[i],
                            row[i].Length));
                }
            }

            return widths;
        }

        private static string BuildRow(
            IReadOnlyList<string> values,
            IReadOnlyList<int> widths)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < values.Count; i++)
            {
                if (i > 0)
                    builder.Append("  ");

                var value = values[i];

                if (i == 0 || long.TryParse(value, out _))
                {
                    builder.Append(
                        value.PadLeft(widths[i]));
                }
                else
                {
                    builder.Append(
                        value.PadRight(widths[i]));
                }
            }

            return builder.ToString();
        }

        private static string BuildSeparator(
            IReadOnlyList<int> widths)
        {
            return string.Join(
                "  ",
                widths.Select(x => new string('-', x)));
        }
    }
}

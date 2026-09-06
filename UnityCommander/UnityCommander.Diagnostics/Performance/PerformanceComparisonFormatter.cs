
namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceComparisonFormatter
        : IPerformanceComparisonFormatter
    {

        public IEnumerable<string> Format(
            string firstName,
            string secondName,
            IReadOnlyList<PerformanceComparison> comparisons)
        {
            if (comparisons.Count == 0)
                yield break;

            var columns = new[]
            {
                "Operation",
                "Metric",
                firstName ?? "Baseline",
                secondName ?? "Baseline#2",
                "Delta"
            };

            var rows = comparisons
                .SelectMany(CreateRows)
                .ToArray();

            var widths = CalculateWidths(
                columns,
                rows);

            yield return BuildRow(
                columns,
                widths);

            yield return BuildSeparator(widths);

            foreach (var row in rows)
                yield return BuildRow(
                    row,
                    widths);
        }

        private static IEnumerable<string[]> CreateRows(
            PerformanceComparison comparison)
        {
            yield return CreateRow(
                comparison,
                "Samples",
                comparison.First.Count,
                comparison.Second.Count,
                isPercentage: true);

            yield return CreateRow(
                comparison,
                "Average",
                comparison.First.Average,
                comparison.Second.Average);

            yield return CreateRow(
                comparison,
                "P95",
                comparison.First.P95,
                comparison.Second.P95);

            yield return CreateRow(
                comparison,
                "Min",
                comparison.First.Min,
                comparison.Second.Min);

            yield return CreateRow(
                comparison,
                "Max",
                comparison.First.Max,
                comparison.Second.Max);

            yield return CreateRow(
                comparison,
                "Total",
                comparison.First.Total,
                comparison.Second.Total);
        }

        private static string[] CreateRow(
            PerformanceComparison comparison,
            string metric,
            object first,
            object second,
            bool isPercentage = false)
        {
            return
            [
                comparison.Operation,
            metric,
            FormatValue(first),
            FormatValue(second),
            FormatDelta(first, second, isPercentage)
            ];
        }

        private static string FormatValue(object value)
        {
            return value switch
            {
                TimeSpan time =>
                    $"{time.TotalMilliseconds:F2} ms",

                _ =>
                    value.ToString() ?? string.Empty
            };
        }

        private static string FormatDelta(
            object first,
            object second,
            bool isPercentage)
        {
            if (isPercentage)
            {
                if (Convert.ToDouble(first) == 0)
                    return "—";

                var delta2 =
                    (Convert.ToDouble(second) -
                     Convert.ToDouble(first)) /
                    Convert.ToDouble(first) *
                    100.0;

                return $"{delta2:+0.0;-0.0;0.0}%";
            }

            var firstMs = ((TimeSpan)first).TotalMilliseconds;

            if (firstMs == 0)
                return "—";

            var secondMs = ((TimeSpan)second).TotalMilliseconds;

            var delta =
                (secondMs - firstMs) /
                firstMs *
                100.0;

            return $"{delta:+0.0;-0.0;0.0}%";
        }

        private static int[] CalculateWidths(
            IReadOnlyList<string> columns,
            IReadOnlyList<string[]> rows)
        {
            var widths = columns
                .Select(x => x.Length)
                .ToArray();

            foreach (var row in rows)
            {
                for (var i = 0; i < row.Length; i++)
                {
                    widths[i] = Math.Max(
                        widths[i],
                        row[i].Length);
                }
            }

            return widths;
        }

        private static string BuildRow(
            IReadOnlyList<string> values,
            IReadOnlyList<int> widths)
        {
            var parts = new string[values.Count];

            for (var i = 0; i < values.Count; i++)
            {
                parts[i] = i switch
                {
                    0 or 1 =>
                        values[i].PadRight(widths[i]),

                    _ =>
                        values[i].PadLeft(widths[i])
                };
            }

            return string.Join("  ", parts);
        }

        private static string BuildSeparator(
            IReadOnlyList<int> widths)
        {
            return string.Join(
                "  ",
                widths.Select(
                    width => new string('-', width)));
        }
    }
}

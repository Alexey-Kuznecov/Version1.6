
namespace UnityCommander.Diagnostics.Workbench.Services
{
    using System.Text.Json;
    using UnityCommander.Diagnostics.Abstraction;
    using UnityCommander.Diagnostics.Workbench.Models;

    public static class TraceEventItemMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static TraceEventItem Map(DiagnosticRecordEntry record)
        {
            try
            {
                using var document = JsonDocument.Parse(record.PayloadJson);

                JsonElement root = document.RootElement;

                string traceId =
                    GetString(root, "traceId")
                    ?? record.Id.ToString("N");

                string? spanId = GetString(root, "spanId");

                string? parentSpanId = GetString(root, "parentSpanId");

                int depth = GetInt32(root, "depth");

                string data = GetData(root);

                double? durationMilliseconds = record.DurationTicks.HasValue
                 ? TimeSpan.FromTicks(record.DurationTicks.Value).TotalMilliseconds
                 : null;

                return new TraceEventItem(
                    timestamp: record.Timestamp.LocalDateTime,
                    eventName: record.Name,
                    source: record.Source,
                    traceId: traceId,
                    depth: depth,
                    durationMilliseconds: durationMilliseconds,
                    data: data,
                    spanId: spanId,
                    parentSpanId: parentSpanId,
                    pluginId: record.PluginId);
            }
            catch (JsonException)
            {
                return new TraceEventItem(
                    timestamp: record.Timestamp.LocalDateTime,
                    eventName: record.Name,
                    source: record.Source,
                    traceId: record.Id.ToString("N"),
                    depth: 0,
                    durationMilliseconds: record.DurationTicks.HasValue
                        ? TimeSpan.FromTicks(record.DurationTicks.Value).TotalMilliseconds
                        : 0,
                    data: record.PayloadJson,
                    pluginId: record.PluginId);
            }
        }

        private static string GetData(JsonElement root)
        {
            if (TryGetProperty(root, "data", out JsonElement data))
            {
                return JsonSerializer.Serialize(data, JsonOptions);
            }

            return JsonSerializer.Serialize(root, JsonOptions);
        }

        private static string? GetString(
            JsonElement element,
            string propertyName)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement value))
            {
                return null;
            }

            return value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;
        }

        private static int GetInt32(
            JsonElement element,
            string propertyName)
        {
            if (TryGetProperty(element, propertyName, out JsonElement value) &&
                value.ValueKind == JsonValueKind.Number &&
                value.TryGetInt32(out int result))
            {
                return result;
            }

            return 0;
        }

        private static bool TryGetProperty(
            JsonElement element,
            string propertyName,
            out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (string.Equals(
                        property.Name,
                        propertyName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }
    }
}

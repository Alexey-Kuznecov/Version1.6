namespace UnityCommander.Diagnostics.Workbench.Models;

using System;
using System.Collections.Generic;
using System.Text.Json;

public sealed class PerformanceMeasurementItem
{
    private static readonly JsonSerializerOptions PrettyJsonOptions = new()
    {
        WriteIndented = true
    };

    public PerformanceMeasurementItem(
        DateTime timestamp,
        double durationMilliseconds,
        string payloadJson)
    {
        Timestamp = timestamp;
        DurationMilliseconds = durationMilliseconds;

        var payload = ParsePayload(payloadJson);

        Operation = payload.Operation ?? "Measurement";
        Sequence = payload.Sequence;
        Metadata = payload.Metadata;
        FormattedRawJson = payload.FormattedJson;
    }

    public DateTime Timestamp { get; }

    public double DurationMilliseconds { get; }

    public string Operation { get; }

    public long? Sequence { get; }

    public string SequenceLabel =>
        Sequence.HasValue
            ? $"Sequence #{Sequence.Value:N0}"
            : "Sequence unavailable";

    public string DurationDisplay =>
        $"{DurationMilliseconds:N2} ms";

    public IReadOnlyList<PerformanceMetadataItem> Metadata { get; }

    public bool HasMetadata => Metadata.Count > 0;

    public string FormattedRawJson { get; }

    private static ParsedPayload ParsePayload(string? payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return new ParsedPayload(
                null,
                null,
                Array.Empty<PerformanceMetadataItem>(),
                "No payload available.");
        }

        try
        {
            using var document = JsonDocument.Parse(payloadJson);

            var root = document.RootElement;

            var operation = ReadString(root, "operation");

            long? sequence = null;

            if (TryGetProperty(root, "sequence", out var sequenceElement) &&
                sequenceElement.ValueKind == JsonValueKind.Number &&
                sequenceElement.TryGetInt64(out var sequenceValue))
            {
                sequence = sequenceValue;
            }

            var metadata = ReadMetadata(root);

            var formattedJson = JsonSerializer.Serialize(
                root,
                PrettyJsonOptions);

            return new ParsedPayload(
                operation,
                sequence,
                metadata,
                formattedJson);
        }
        catch (JsonException)
        {
            return new ParsedPayload(
                null,
                null,
                Array.Empty<PerformanceMetadataItem>(),
                payloadJson);
        }
    }

    private static IReadOnlyList<PerformanceMetadataItem> ReadMetadata(
        JsonElement root)
    {
        if (!TryGetProperty(root, "metadata", out var metadataElement) ||
            metadataElement.ValueKind != JsonValueKind.Object)
        {
            return Array.Empty<PerformanceMetadataItem>();
        }

        var result = new List<PerformanceMetadataItem>();

        foreach (var property in metadataElement.EnumerateObject())
        {
            result.Add(
                new PerformanceMetadataItem(
                    property.Name,
                    FormatValue(property.Value)));
        }

        return result;
    }

    private static string? ReadString(
        JsonElement element,
        string propertyName)
    {
        if (!TryGetProperty(element, propertyName, out var value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static string FormatValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String =>
                value.GetString() ?? string.Empty,

            JsonValueKind.Number =>
                value.GetRawText(),

            JsonValueKind.True =>
                "True",

            JsonValueKind.False =>
                "False",

            JsonValueKind.Null =>
                "null",

            JsonValueKind.Object or JsonValueKind.Array =>
                JsonSerializer.Serialize(value, PrettyJsonOptions),

            _ => value.GetRawText()
        };
    }

    private static bool TryGetProperty(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty(propertyName, out value))
            {
                return true;
            }

            foreach (var property in element.EnumerateObject())
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

    private sealed record ParsedPayload(
        string? Operation,
        long? Sequence,
        IReadOnlyList<PerformanceMetadataItem> Metadata,
        string FormattedJson);
}

public sealed record PerformanceMetadataItem(
    string Name,
    string Value);

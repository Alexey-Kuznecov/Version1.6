
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnityCommander.Diagnostics.Storage.Serialization;

public sealed class DiagnosticJsonSerializer
{
    private readonly JsonSerializerOptions _options;

    public DiagnosticJsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        _options.Converters.Add(
            new JsonStringEnumConverter());
    }

    public string Serialize(object? payload)
    {
        if (payload is null)
        {
            return "null";
        }

        try
        {
            return JsonSerializer.Serialize(
                payload,
                payload.GetType(),
                _options);
        }
        catch (Exception exception) when (
            exception is JsonException ||
            exception is NotSupportedException ||
            exception is InvalidOperationException)
        {
            return JsonSerializer.Serialize(new
            {
                SerializationError = exception.Message,
                PayloadType = payload.GetType().FullName
            });
        }
    }
}

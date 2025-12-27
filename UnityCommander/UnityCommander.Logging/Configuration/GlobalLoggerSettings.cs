namespace UnityCommander.Logging.Configuration
{
    public sealed class GlobalLoggerSettings
    {
        public LoggingMode Mode { get; init; }
        public LogLevel MinimumLevel { get; init; }

        public IReadOnlySet<string>? EnabledCategories { get; init; }
        public IReadOnlySet<string>? EnabledScopes { get; init; }
    }
}

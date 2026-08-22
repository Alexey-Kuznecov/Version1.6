
namespace UnityCommander.Logging.Configuration
{
    public sealed class LoggingSettings
    {
        public HashSet<string> DisabledCategories { get; set; } = new();

        public HashSet<string> DisabledScopes { get; set; } = new();

        public HashSet<LogLevel> DisabledLevels { get; set; } = new();

        public bool ProfilingEnabled { get; set; } = true;

        public double? MinimumProfileDurationMs { get; set; }

        public LogLevel? MinimumLevelOverride { get; set; }
    }
}

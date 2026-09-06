
using UnityCommander.Logging.Configuration;

namespace UnityCommander.Logging.Infrastructure
{
    public interface ILoggingRuntimeControl
    {
        IReadOnlySet<string> DisabledCategories { get; }
        IReadOnlySet<string> DisabledScopes { get; }
        IReadOnlySet<LogLevel> DisabledLevels { get; }

        LogLevel? MinimumLevelOverride { get; }

        bool ProfilingEnabled { get; }
        double? MinimumProfileDurationMs { get; }

        bool IsLevelEnabled(LogLevel level);
        bool IsCategoryEnabled(string category);
        bool IsScopeEnabled(string scope);

        void EnableLevel(LogLevel level);
        void DisableLevel(LogLevel level);

        void EnableCategory(string category);
        void DisableCategory(string category);
        
        void EnableScope(string scope);
        void DisableScope(string scope);

        void EnableProfiling();
        void DisableProfiling();

        void SetMinimumProfileDuration(double milliseconds);

        void Reset();
    }
}

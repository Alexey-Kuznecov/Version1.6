
using UnityCommander.Logging.Configuration;

namespace UnityCommander.Logging.Infrastructure
{
    public sealed class LoggingRuntimeControl : ILoggingRuntimeControl
    {
        private readonly ILoggingSettingsStore _store;
        private readonly LoggingSettings _settings;

        public LoggingRuntimeControl(
            ILoggingSettingsStore store)
        {
            _store = store;
            _settings = store.Load();
        }

        public IReadOnlySet<string> DisabledCategories
            => _settings.DisabledCategories;

        public IReadOnlySet<string> DisabledScopes
            => _settings.DisabledScopes;

        public IReadOnlySet<LogLevel> DisabledLevels
            => _settings.DisabledLevels;

        public bool ProfilingEnabled
            => _settings.ProfilingEnabled;

        public double? MinimumProfileDurationMs
            => _settings.MinimumProfileDurationMs;

        public LogLevel? MinimumLevelOverride
            => _settings.MinimumLevelOverride;

        public bool IsLevelEnabled(LogLevel level)
          => !_settings.DisabledLevels.Contains(level);

        public bool IsCategoryEnabled(string category)
         => !_settings.DisabledCategories.Contains(category);

        public bool IsScopeEnabled(string scope)
            => !_settings.DisabledScopes.Contains(scope);

        public void DisableLevel(LogLevel level)
        {
            if (_settings.DisabledLevels.Add(level))
                Save();
        }

        public void EnableLevel(LogLevel level)
        {
            if (_settings.DisabledLevels.Remove(level))
                Save();
        }

        public void EnableCategory(string category)
        {
            if (_settings.DisabledCategories.Remove(category))
                Save();
        }

        public void DisableCategory(string category)
        {
            if (_settings.DisabledCategories.Add(category))
                Save();
        }

        public void EnableScope(string scope)
        {
            if (_settings.DisabledScopes.Remove(scope))
                Save();
        }

        public void DisableScope(string scope)
        {
            if (_settings.DisabledScopes.Add(scope))
                Save();
        }

        public void EnableProfiling()
        {
            if (!_settings.ProfilingEnabled)
            {
                _settings.ProfilingEnabled = true;
                Save();
            }
        }

        public void DisableProfiling()
        {
            if (_settings.ProfilingEnabled)
            {
                _settings.ProfilingEnabled = false;
                Save();
            }
        }

        public void SetMinimumProfileDuration(double milliseconds)
        {
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds));

            if (_settings.MinimumProfileDurationMs != milliseconds)
            {
                _settings.MinimumProfileDurationMs = milliseconds;
                Save();
            }
        }

        public void Reset()
        {
            _settings.DisabledCategories.Clear();
            _settings.DisabledScopes.Clear();
            _settings.DisabledLevels.Clear();

            _settings.ProfilingEnabled = true;
            _settings.MinimumProfileDurationMs = null;
            _settings.MinimumLevelOverride = null;

            Save();
        }

        private void Save()
        {
            _store.Save(_settings);
        }
    }
}

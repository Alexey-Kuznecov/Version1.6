
namespace UnityCommander.Logging.Configuration
{
    public interface ILoggingSettingsStore
    {
        LoggingSettings Load();
        void Save(LoggingSettings settings);
    }
}

using System;
using UnityCommander.Logging.Configuration;

namespace UnityCommander.Services.Interfaces.Settings
{
    public interface ILoggingSettingsService
    {
        GlobalLoggerSettings Current { get; }
        event Action SettingsChanged;
    }
}

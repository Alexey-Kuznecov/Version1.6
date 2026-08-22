
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Filters;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Logging.Sinks;
using UnityCommander.Services;
using UnityCommander.Settings.Abstactions;
using UnityCommander.Settings.Core;
using UnityCommander.Sinks;

namespace UnityCommander.Dependencies
{
    public static class LoggingModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            var settings = new GlobalLoggerSettings
            {
                Mode = LoggingMode.Debug,
                MinimumLevel = LogLevel.Debug,
                EnabledScopes = new HashSet<string>
                {
                    "Startup",
                    "Runtime"
                },
                EnabledCategories = new HashSet<string>
                {
                    "System",
                    "Plugin"
                }
            };

            registry.RegisterInstance(settings);

            registry.RegisterSingleton<LogHub>();
            registry.RegisterSingleton<ILogSink, NullSink>();
            registry.RegisterSingleton<ILogSink>(_ => new FileLogSink("journal.log", LogChannel.Journal));
            registry.RegisterSingleton<ILogSink>(_ => new FileLogSink("errors.log", LogChannel.Error));
            registry.RegisterSingleton<ILogFilter, LoggingPolicyFilter>();
            registry.RegisterSingleton<ILogColorResolver, DefaultLogColorResolver>();
            registry.RegisterSingleton<ILoggingRuntimeControl, LoggingRuntimeControl>();

            registry.RegisterSingleton<ILoggingSettingsStore>(sp =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "config", "logging.json"); ;
                return new JsonLoggingSettingsStore(path);
            });

            registry.RegisterSingleton<LoggerCreator>();
            registry.RegisterSingleton<LoggingSinkService>();
        }
    }
}

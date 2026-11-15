
using Prism.Ioc;
using System.IO;
using UnityCommander.Commands.Diagnostic;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Performance;
using UnityCommander.Commands.Rendering;
using UnityCommander.Commands.Services;
using UnityCommander.Common;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Core.Diagnostics;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Abstraction.Performance;
using UnityCommander.Diagnostics.Performance;
using UnityCommander.Diagnostics.Reporting;
using UnityCommander.Diagnostics.Storage;
using UnityCommander.Diagnostics.Storage.SQLite;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Services.Diagnostics;

namespace UnityCommander.Dependencies
{
    public static class DiagnosticRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Сервисы, используемые командами внутренней консоли
            registry.RegisterSingleton<ISysStatService, SysStatService>();
            registry.RegisterSingleton<IProcessOpenFilesService, ProcessOpenFilesService>();

            // Вспомогательные компоненты инфраструктуры команд
            registry.RegisterSingleton<ICommandArgumentParser, CommandArgumentParser>();
            registry.RegisterSingleton<IConsoleRenderer<SystemStats>, SystemStatsRenderer>();

            registry.RegisterSingleton<IDiagnosticRender, DiagnosticRender>();
            registry.RegisterSingleton<IDiagnosticPipeline, DiagnosticPipeline>();
            registry.RegisterSingleton<IDiagnosticRegistry, DiagnosticRegistry>();

            registry.RegisterSingleton<IPerformanceAnalyzer, PerformanceAnalyzer>();
            registry.RegisterSingleton<IPerformanceProfiler, PerformanceProfiler>();
            registry.RegisterSingleton<IPerformanceSnapshotService, PerformanceSnapshotService>();
            registry.RegisterSingleton<IPerformanceComparisonService, PerformanceComparisonService>();


            registry.RegisterSingleton<IDiagnosticTraceStore, DiagnosticTraceStore>();

            registry.RegisterSingleton<IDiagnosticReportingService, DiagnosticReportingService>();

            registry.RegisterSingleton<IDiagnosticTrace, Diagnostics.Tracing.DiagnosticTrace>();

#if UNITYCOMMANDER_DIAGNOSTICS

            registry.RegisterSingleton<IDiagnosticsStorage, SqliteDiagnosticsStorage>();
            registry.RegisterSingleton<IDiagnosticsRecorder, DiagnosticsRecorder>();

            registry.RegisterSingleton<SqliteConnectionFactory>(sp =>
            {
                var paths = sp.Resolve<UnityCommanderPath>();

                return new SqliteConnectionFactory(
                    Path.Combine(paths.DiagnosticDirectory, "diagnostics.db"));
            });

#endif
        }
    }
}

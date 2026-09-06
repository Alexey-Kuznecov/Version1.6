
using AdvancedCopyFiles.Commands;
using AdvancedCopyFiles.Core;
using AdvancedCopyFiles.Services;
using AdvancedCopyFiles.ViewModels;
using AdvancedCopyFiles.Views;
using CommandSystem.CopyTester.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Dialog;
using UnityCommander.Copying;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Strategies;
using UnityCommander.Core.Plugin;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Ribbon.Abstractions.Models;

[assembly: PluginInfo(
    name: "Advanced Copy",
    developerId: "advance-copy-1.0",
    author: "UnityCommander Team",
    version: "1.0",
    description: "Продвинутый копировщик файлов"
)]
namespace AdvancedCopyFiles
{
    public class Plugin : IPlugin, IDisposable
    {
        private ILogger _logger;

        public string Name => "Advanced Copy";

        public string Version => "1.0";

        public void Initialize(IPluginInitContext init)
        {
            init.RegisterCommand(new CommandDefinition()
            {
                Id = "open-copy-advanced",
                IconKey = "core.file",
                CommandType = typeof(OpenCopyWindowCommand),
            });

            init.RegisterCommand(new CommandDefinition()
            {
                Id = "open-old-copy-advanced",
                IconKey = "core.file",
                CommandType = typeof(OpenCopyDialog),
            });

            init.ConfigureRibbon(r =>
            {
                r.Tab("home", "Главная")
                    .Group("clipboard", "Буфер")
                        .Section("main", RibbonGroupLayout.Large)
                            .Button("open-copy-advanced", "git")
                            .Button("open-old-copy-advanced", "git");
                    //    .EndSection()
                    //.EndGroup()
                    //.Group("tools", "Инструменты")
                    //    .Section("plugins", RibbonGroupLayout.Inline)
                    //        .Button("advanced-copy", "advanced-copy");

                //r.Tab("view", "Вид")
                //    .Group("panels", "Панели")
                //        .Section("main", RibbonGroupLayout.Medium)
                //            .Button("toggle-sidebar", "toggle-sidebar")
                //            .Button("toggle-console", "toggle-console");
            });

            init.RegisterSingleton<IProgressCalculator, ProgressCalculator>();
            init.RegisterSingleton<ISpeedCalculator, SpeedCalculator>();
            init.RegisterSingleton<IProgressTracker, ProgressTracker>();

            init.RegisterSingleton<GuiProgressReporter>();

            init.RegisterSingleton(sp =>
            {
                var gui = sp.GetRequiredService<GuiProgressReporter>();

                return new AggregatedProgressReporter(
                    gui,
                    TimeSpan.FromMilliseconds(1));
            });

            init.RegisterSingleton<IProgressReporter>(sp =>
                sp.GetRequiredService<AggregatedProgressReporter>());

            init.RegisterSingleton<IFileCategorizer, NeuralCategorizer>();
            init.RegisterSingleton<IFileCopyPlanner, DefaultFileCopyPlanner>();

            init.RegisterSingleton<ICopyExecutionStrategy, ParallelExecutionStrategy>();
            init.RegisterSingleton<IFileCopierFactory, DefaultFileCopierFactory>();

            init.RegisterSingleton<ICopyExecutor, CopyExecutor>();
            init.RegisterSingleton<ICopyMetricsCollector, NullCopyMetricsCollector>();

            init.RegisterSingleton<ICopyReporter, CopyLogReporter>();
            init.RegisterSingleton<ICopyReporter, CopyFileReporter>();

            init.RegisterSingleton<OpenManager>();
            //init.RegisterSingleton<CopyManager>();
            init.RegisterSingleton<CopySessionManager>();
            init.RegisterSingleton<ICopySettingsBuilder, CopySettingsBuilder>();

            init.RegisterTransient<SettingsViewModel>();
            init.RegisterTransient<ProgressViewModel>();
            init.RegisterTransient<FileListViewModel>();
            init.RegisterTransient<SpeedGraphViewModel>();
            init.RegisterTransient<HistoryViewModel>();
            init.RegisterTransient<MetricViewModel>();
            init.RegisterTransient<LogViewModel>();

            init.ConfigureComposition<MainView>(b =>
            {
                b.Add<SettingsView, SettingsViewModel>("SettingsVM");
                b.Add<ProgressView, ProgressViewModel>("ProgressVM");
                b.Add<FileListView, FileListViewModel>("FileListVM");
                b.Add<SpeedGraph, SpeedGraphViewModel>("SpeedGraphVM");
                b.Add<MetricView, MetricViewModel>("MetricVM");
                b.Add<HistoryView, HistoryViewModel>("HistoryVM");
                b.Add<LogView, LogViewModel>("LogVM");
            });

            init.RegisterDialog(new DialogDefinition(
                "advance-copy-1.0",
                typeof(MainViewOld),
                typeof(CopyWindowViewModel),
                new DialogOptions()
                {
                    Height = 900,
                    Width = 1050,
                    Title = "Продвинутый копировщик файлов",
                    IsResizable = false,
                }));

            init.RegisterOverride<IFileOperationService, AdvancedFileCopyEngine>();
        }

        public void Start(IPluginContext context)
        {
            var loggerCreate = context.Services.Get<LoggerCreator>();
            _logger = loggerCreate.ForPlugin();
            
            _logger.Info($"{Name} is ready!!!");
        }

        public void Stop()
        {
            _logger = null;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}

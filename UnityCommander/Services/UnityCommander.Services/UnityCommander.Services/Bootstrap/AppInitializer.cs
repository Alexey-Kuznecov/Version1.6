
using AvalonDock.Layout;
using CommandSystem.Core.UndoRedo;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.History;
using UnityCommander.CLI.History;
using UnityCommander.Common.Docking;
using UnityCommander.Common.State;
using UnityCommander.Diagnostics.Reporting;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Bootstrap
{
    public class AppInitializer
    {
        private AppSessionState _state;
        private readonly ISessionService _session;
        private readonly ILayoutService _layout;
        private readonly IPanelService _panel;
        private readonly ISessionBuilder _builder;
        private readonly IDockingSyncService _dockingSync;
        private readonly ISessionAggregator _sessionAggregator;
        private readonly SessionStateValidator _stateValidator;
        private readonly IToolDockingStore _toolDockingStore;
        private readonly ConsoleHistoryService _consoleHistory;
        private readonly ILogger _logger;
        private readonly LoggerCreator _loggerCreator;
        private readonly IUserNavigationHistory _navigationHistory;
        private readonly IUserFavorites _userFavorites;
        private readonly IUserFavoriteStore _favoriteStore;
        private readonly IUserNavigationHistoryStore _userNavigation;

        private readonly IDiagnosticReportingService _reporting;

        public AppInitializer(
            ISessionService session,
            ILayoutService layout,
            IPanelService panel,
            ISessionBuilder builder, 
            IDockingSyncService dockingSync,
            ISessionAggregator sessionAggregator, 
            IMultiCommandService multiCommand,
            IToolDockingStore toolDockingStore,
            IUserNavigationHistory userNavigationHistory,
            IUserFavorites userFavorites,
            IUserNavigationHistoryStore navigationHistoryStore,
            IUserFavoriteStore userFavoriteStore,
            ConsoleHistoryService consoleHistory,
            LoggerCreator logger, 
            SessionStateValidator stateValidator, 
            IDiagnosticReportingService reporting) 
        {
            _reporting = reporting;

            _loggerCreator = logger;
            _stateValidator = stateValidator;

            _logger = _loggerCreator.For<AppInitializer>(
               scope: LogScope.Startup
            );

            _session = session;
            _layout = layout;
            _panel = panel;
            _builder = builder;
            _dockingSync = dockingSync;
            _sessionAggregator = sessionAggregator;
            _consoleHistory = consoleHistory;
            _toolDockingStore = toolDockingStore;
            _navigationHistory = userNavigationHistory;
            _userFavorites = userFavorites;
            _userNavigation = navigationHistoryStore;
            _favoriteStore = userFavoriteStore;

            multiCommand.SaveCommand.RegisterCommand(SavePanelStateCommand);
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
        () =>
        {
            _builder.Build(_state);   // 💥 СОБИРАЕМ

            _sessionAggregator.Capture(_state);

            _session.Save(_state);

            _layout.Save();

            _toolDockingStore.Save();

            _consoleHistory.Save();

            _favoriteStore.Save(_userFavorites.Paths);

            _userNavigation.Save(_navigationHistory.Paths);
            
        });

        public void Initialize()
        {
            var started = System.Diagnostics.Stopwatch.GetTimestamp();

            _reporting.Publish(
                new DiagnosticEvent(
                    "Application.Startup",
                    "Initialize.Started"));

            try
            {
                using (_loggerCreator.ProfileScope(
                    LogScope.Startup,
                    "Layout Initial"))
                {
                    _logger.Info("Session load..");

                    RunStartupStep(
                        "Session.Load",
                        () =>
                        {
                            _state = _session.Load();
                        });

                    RunStartupStep(
                        "Session.Validate",
                        () =>
                        {
                            _stateValidator.Validate(_state);
                        });

                    _logger.Info("AvalonDock init..");

                    RunStartupStep(
                        "Layout.Load",
                        () =>
                        {
                            _layout.Load(_state);
                        });

                    _logger.Info("Initial Panel..");

                    RunStartupStep(
                        "Panels.Initialize",
                        () =>
                        {
                            _panel.Initialize();
                        });

                    _logger.Info("AvalonDock and Panel sync..");

                    RunStartupStep(
                        "Docking.Synchronize",
                        () =>
                        {
                            _dockingSync.Initialize(_state.Panels);
                        });

                    _logger.Info("Restore prev session..");

                    RunStartupStep(
                        "ToolLayout.Load",
                        () =>
                        {
                            _toolDockingStore.Load();
                        });

                    _logger.Info("Tool layout loaded.");

                    RunStartupStep(
                        "Session.Restore",
                        () =>
                        {
                            _sessionAggregator.Restore(_state);
                        });

                    RunStartupStep(
                        "ConsoleHistory.Initialize",
                        () =>
                        {
                            _consoleHistory.Initialize();
                        });

                    _logger.Info("Console history initialized..");
                }

                _reporting.Publish(
                    new DiagnosticEvent(
                        "Application.Startup",
                        "Initialize.Completed",
                        Duration: System.Diagnostics.Stopwatch.GetElapsedTime(
                            started)));
            }
            catch (Exception exception)
            {
                _reporting.Publish(
                    new DiagnosticEvent(
                        "Application.Startup",
                        "Initialize.Failed",
                        new
                        {
                            ExceptionType = exception.GetType().FullName,
                            exception.Message
                        },
                        Duration: System.Diagnostics.Stopwatch.GetElapsedTime(
                            started)));

                throw;
            }
        }

        private void RunStartupStep(string name, Action action)
        {
            var started = System.Diagnostics.Stopwatch.GetTimestamp();

            try
            {
                action();

                var duration =
                    System.Diagnostics.Stopwatch.GetElapsedTime(started);

                _reporting.Publish(
                    new DiagnosticEvent(
                        "Application.Startup",
                        name,
                        new
                        {
                            Status = "Completed"
                        },
                        Duration: duration));
            }
            catch (Exception exception)
            {
                var duration =
                    System.Diagnostics.Stopwatch.GetElapsedTime(started);

                _reporting.Publish(
                    new DiagnosticEvent(
                        "Application.Startup",
                        name,
                        new
                        {
                            Status = "Failed",
                            ExceptionType = exception.GetType().FullName,
                            exception.Message
                        },
                        Duration: duration));

                throw;
            }
        }
    }
}

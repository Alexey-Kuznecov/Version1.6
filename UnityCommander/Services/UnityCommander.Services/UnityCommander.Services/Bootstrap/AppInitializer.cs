
using Prism.Commands;
using UnityCommander.CLI.History;
using UnityCommander.Common.Docking;
using UnityCommander.Common.State;
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
        private readonly IToolDockingService _toolDocking;
        private readonly ConsoleHistoryService _consoleHistory;
        private readonly ILogger _logger;
        private readonly LoggerCreator _loggerCreator;

        public AppInitializer(
            ISessionService session,
            ILayoutService layout,
            IPanelService panel,
            ISessionBuilder builder, 
            IDockingSyncService dockingSync,
            ISessionAggregator sessionAggregator, 
            IMultiCommandService multiCommand,
            IToolDockingService toolDocking,
            ConsoleHistoryService consoleHistory,
            LoggerCreator logger) 
        {
            _loggerCreator = logger;
            
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
            _toolDocking = toolDocking;

            multiCommand.SaveCommand.RegisterCommand(SavePanelStateCommand);
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
        () =>
        {
            _builder.Build(_state);   // 💥 СОБИРАЕМ

            _sessionAggregator.Capture(_state);

            _session.Save(_state);

            _layout.Save();

            _toolDocking.Save();

            _consoleHistory.Save();
        });

        public void Initialize()
        {
            using (_loggerCreator.ProfileScope(LogScope.Startup, "Layout Initial"))
            {
                _logger.Info("Session load..");
                _state = _session.Load();

                _logger.Info("AvalonDock init..");
                _layout.Load(_state);

                _logger.Info("Initial Panel..");
                _panel.Initialize();

                _logger.Info("AvalonDock and Panel sync..");
                _dockingSync.Initialize(_state.Panels);

                _logger.Info("Restore prev session..");
                _sessionAggregator.Restore(_state);

                _toolDocking.Load();
                _logger.Info("Tool layout loaded.");

                 _consoleHistory.Initialize();
                _logger.Info("Console history initialized..");
            }
        }
    }
}

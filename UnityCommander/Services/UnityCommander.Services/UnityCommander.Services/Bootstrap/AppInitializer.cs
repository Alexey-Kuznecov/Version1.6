
using Prism.Commands;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Common.Docking;
using UnityCommander.Common.State;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Logging.Profiling;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;

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

        private LogHub _hub;
        private readonly ILogger _logger;

        public AppInitializer(
            ISessionService session,
            ILayoutService layout,
            IPanelService panel,
            ISessionBuilder builder, 
            IDockingSyncService dockingSync,
            ISessionAggregator sessionAggregator, 
            IMultiCommandService multiCommand, 
            LogHub hub, // Profiled as AppInitializer, but it is not a hot path, so we can ignore it for now
            LoggerCreator logger
            ) 
        {
            _logger = logger.For<AppInitializer>(
               scope: LogScope.Startup
            );

            _hub = hub;
            _session = session;
            _layout = layout;
            _panel = panel;
            _builder = builder;
            _dockingSync = dockingSync;
            _sessionAggregator = sessionAggregator;

            multiCommand.SaveCommand.RegisterCommand(SavePanelStateCommand);
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
        () =>
        {
            _builder.Build(_state);   // 💥 СОБИРАЕМ

            _sessionAggregator.Capture(_state);

            _session.Save(_state);

            _layout.Save();
        });

        public void Initialize()
        {
            using (new LogScopeTimer(_hub, LogScope.Startup, "Layout Initial"))
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
            }
        }
    }
}

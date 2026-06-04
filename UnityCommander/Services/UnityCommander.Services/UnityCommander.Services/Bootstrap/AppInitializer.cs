
using Prism.Commands;
using Prism.Ioc;
using UnityCommander.Common.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;

namespace UnityCommander.Services.Bootstrap
{
    public class AppInitializer
    {
        private readonly ISessionService _session;
        private readonly ILayoutService _layout;
        private readonly IPanelService _panel;
        private readonly ISessionBuilder _builder;
        private readonly IDockingSyncService _dockingSync;
        private readonly ISessionAggregator _sessionAggregator;

        public AppInitializer(
            ISessionService session,
            ILayoutService layout,
            IPanelService panel,
            ISessionBuilder builder, 
            IDockingSyncService dockingSync,
            ISessionAggregator sessionAggregator, 
            IMultiCommandService multiCommand)
        {
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
            var state = _builder.Build();   // 💥 СОБИРАЕМ

            _sessionAggregator.Capture(state);

            _session.Save(state);

            _layout.Save();
        });

        public void Initialize()
        {
            var session = _session.Load();

            _layout.Load(session);

            _panel.Initialize();

            _dockingSync.Initialize(session.Panels);

            _sessionAggregator.Restore(session);
        }
    }
}

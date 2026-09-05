
using Example;
using Prism.Ioc;
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Abstractions.Sidebar;
using UnityCommander.Common;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Sidebar;
using UnityCommander.Core.Background;
using UnityCommander.Core.Panels;
using UnityCommander.Core.Registrar;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Indexing;
using UnityCommander.Index.Storage;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.StatusBar.Services;
using UnityCommander.Modules.ToolBar.Builder;
using UnityCommander.Rendering.Icons;
using UnityCommander.Rendering.Icons.Services;
using UnityCommander.Rendering.Icons.Strategies;
using UnityCommander.Ribbon.Services;
using UnityCommander.Ribbon.Services.Icon;
using UnityCommander.Services;
using UnityCommander.Services.Background;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Docking;
using UnityCommander.Services.Interfaces.Sidebar;
using UnityCommander.Services.Layout;
using UnityCommander.WPF;
using UnityCommander.WPF.Input;

namespace UnityCommander.Dependencies
{
    public static class AppInfrastructureRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<UnityCommanderPath>();
            registry.RegisterSingleton<IEventBus, EventBus>();

            // Компоновка UI: отвечает за то, как строятся и наполняются области интерфейса (панели/лейауты)
            registry.RegisterSingleton<ILayoutService, LayoutService>();
            registry.RegisterSingleton<ILayoutContentFactory, PanelContentFactory>();

            // Сессия приложения: хранит и собирает текущее состояние работы пользователя (открытые панели, контекст и т.д.)
            registry.RegisterSingleton<ISessionService, SessionService>();
            registry.RegisterSingleton<ISessionAggregator, SessionAggregator>();
            registry.RegisterSingleton<ISessionBuilder, SessionBuilder>();
            registry.RegisterSingleton<SessionStateValidator>();

            // Управление панелями: создание, жизненный цикл и управление UI-панелями
            registry.RegisterSingleton<IPanelService, PanelService>();

            // Реестры UI: глобальное хранение вкладок, панелей и доступ к текущему UI-контексту
            registry.RegisterSingleton<ITabRegistry, TabRegistry>();
            registry.RegisterSingleton<IPanelRegistry, PanelRegistry>();
            registry.RegisterSingleton<ITabContextAccessor, TabContextAccessor>();
            registry.RegisterSingleton<ITabActivationService, TabActivationService>();

            // Docking (перетаскивание UI): логика докинга, синхронизация и общий контекст перемещения панелей
            registry.RegisterSingleton<IDockingService, DockingService>();
            registry.RegisterSingleton<IDockingSyncService, DockingSyncService>();
            registry.RegisterSingleton<DockingSyncContext>();

            // Настройки приложения: конфиги и пользовательские настройки
            //registry.RegisterSingleton<IAppConfigService, AppConfigService>();
            //registry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();

            // UI-лента (Ribbon): управление кнопками/командами верхнего меню
            registry.RegisterSingleton<IRibbonManager, RibbonManager>();
            registry.RegisterSingleton<IRibbonIconProvider, RibbonIconProvider>();

            registry.RegisterSingleton<ISidebarSectionFactory, SidebarSectionFactory>();
            registry.RegisterSingleton<IViewResolver, ViewResolver> ();
            registry.RegisterSingleton<ISidebarService, SidebarService>();
            registry.RegisterSingleton<ISidebarRegistry, SidebarRegistry>();
            registry.RegisterSingleton<ISidebarSectionFactory, SidebarSectionFactory>();

            // Ресурсы интерфейса: поставка иконок и визуальных элементов
            registry.RegisterSingleton<IIconSourceRegistry, IconSourceRegistry>();
            registry.RegisterSingleton<IIconRenderService, IconRenderService>();
            registry.RegisterSingleton<IIconBrushResolver, IconBrushResolver>();
            registry.RegisterSingleton<IIconRenderStrategyResolver, IconRenderStrategyResolver>();
            registry.RegisterSingleton<IIconRenderStrategy, StrokeIconRenderStrategy>();
            registry.RegisterSingleton<IIconRenderStrategy, LayeredIconRenderStrategy>();
            registry.RegisterSingleton<IIconRenderStrategy, FilledIconRenderStrategy>();
            registry.RegisterSingleton<IIconRenderNormalizer, IconRenderNormalizer>();
            registry.RegisterSingleton<IIconResolver, CompositeIconResolver>();
            registry.RegisterSingleton<IIconColorResolver, IconColorResolver>();

            // Менеджер лейаута оболочки: отвечает за общую структуру интерфейса, области и их наполнение
            registry.RegisterSingleton<IShellLayoutManager, ShellLayoutManager>();

            registry.RegisterSingleton<IServiceScopeResolver, ServiceScopeResolver>();
            registry.RegisterSingleton<IServiceOverrideRegistry, ServiceOverrideRegistry>();
            registry.RegisterSingleton<ServiceOverrideResolver>();

            // Ribbon
            registry.RegisterSingleton<IRibbonModelFactory, RibbonModelFactory>();
            registry.RegisterSingleton<IRibbonRegistry, RibbonRegistry>();

            registry.RegisterSingleton<IBackgroundServiceRegistry, BackgroundServiceRegistry>();

            registry.RegisterSingleton<IInputCaptureManager, InputCaptureManager>();
            registry.RegisterSingleton<IInputContextService, InputContextService>();
            registry.RegisterSingleton<IInputState, InputState>();
            registry.RegisterSingleton<IInputRouter, InputRouter>();
            //registry.RegisterSingleton<WindowInputHook>();

            // Старт приложения: точка инициализации всей системы при запуске
            registry.RegisterSingleton<AppInitializer>();

            registry.RegisterSingleton<IBackgroundService, ColumnRefreshService>();
            registry.RegisterSingleton<IBackgroundService, DirectoryChangeService>();
            registry.RegisterSingleton<IBackgroundService, CopyMonitorService>();

            registry.RegisterSingleton<BackgroundServiceHost>();

            registry.RegisterSingleton<IViewFactory, ViewFactory>();
            registry.RegisterSingleton<IViewRegistry, ViewRegistry>();
            registry.RegisterSingleton<IPopupService, PopupService>();

            registry.RegisterSingleton<ICursorTargetService, CursorTargetService>();
            registry.RegisterSingleton<IProgressIndicatorService, ProgressIndicatorService>();

            registry.RegisterSingleton<IUserActivityService, UserActivityService>();
            registry.RegisterSingleton<IBackgroundResourcePolicy, BackgroundResourcePolicy>();
            registry.RegisterSingleton<IBackgroundWorkController, CopyBackgroundWorkController>();

            registry.RegisterSingleton<SqliteFileIndex>(sp =>
            {
                var paths = sp.Resolve<UnityCommanderPath>();

                return new SqliteFileIndex(
                    Path.Combine(
                        paths.DataDirectory,
                        "file_index.db"));
            });

            registry.RegisterSingleton<IFileIndexReader>(sp =>
                sp.Resolve<SqliteFileIndex>());

            registry.RegisterSingleton<IFileIndexWriter>(sp =>
                sp.Resolve<SqliteFileIndex>());

            registry.RegisterSingleton<IFileIndexService, FileIndexService>();
            registry.RegisterSingleton<IFileIndexChangeQueue, FileIndexChangeQueue>();
            registry.RegisterSingleton<IFileIndexSynchronizer, FileIndexSynchronizer>();
        }
    }
}

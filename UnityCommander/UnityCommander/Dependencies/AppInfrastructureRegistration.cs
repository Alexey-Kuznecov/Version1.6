
using Prism.Ioc;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Abstractions.Sidebar;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Sidebar;
using UnityCommander.Core.Background;
using UnityCommander.Core.Panels;
using UnityCommander.Core.Registrar;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.StatusBar.Services;
using UnityCommander.Modules.ToolBar.Builder;
using UnityCommander.Rendering.Icons;
using UnityCommander.Ribbon.Services;
using UnityCommander.Services;
using UnityCommander.Services.Background;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Sidebar;
using UnityCommander.Services.Layout;
using UnityCommander.WPF;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Dependencies
{
    public static class AppInfrastructureRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IEventBus, EventBus>();

            // Компоновка UI: отвечает за то, как строятся и наполняются области интерфейса (панели/лейауты)
            registry.RegisterSingleton<ILayoutService, LayoutService>();
            registry.RegisterSingleton<ILayoutContentFactory, PanelContentFactory>();

            // Сессия приложения: хранит и собирает текущее состояние работы пользователя (открытые панели, контекст и т.д.)
            registry.RegisterSingleton<ISessionService, SessionService>();
            registry.RegisterSingleton<ISessionAggregator, SessionAggregator>();
            registry.RegisterSingleton<ISessionBuilder, SessionBuilder>();

            // Управление панелями: создание, жизненный цикл и управление UI-панелями
            registry.RegisterSingleton<IPanelService, PanelService>();

            // Реестры UI: глобальное хранение вкладок, панелей и доступ к текущему UI-контексту
            registry.RegisterSingleton<ITabRegistry, TabRegistry>();
            registry.RegisterSingleton<IPanelRegistry, PanelRegistry>();
            registry.RegisterSingleton<ITabContextAccessor, TabContextAccessor>();

            // Docking (перетаскивание UI): логика докинга, синхронизация и общий контекст перемещения панелей
            registry.RegisterSingleton<IDockingService, DockingService>();
            registry.RegisterSingleton<IDockingSyncService, DockingSyncService>();
            registry.RegisterSingleton<DockingSyncContext>();

            // Настройки приложения: конфиги и пользовательские настройки
            //registry.RegisterSingleton<IAppConfigService, AppConfigService>();
            //registry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();

            // UI-лента (Ribbon): управление кнопками/командами верхнего меню
            registry.RegisterSingleton<IRibbonManager, RibbonManager>();
            
            registry.RegisterSingleton<ISidebarSectionFactory, SidebarSectionFactory>();
            registry.RegisterSingleton<IViewResolver, ViewResolver> ();
            registry.RegisterSingleton<ISidebarService, SidebarService>();
            registry.RegisterSingleton<ISidebarRegistry, SidebarRegistry>();
            registry.RegisterSingleton<ISidebarSectionFactory, SidebarSectionFactory>();

            // Ресурсы интерфейса: поставка иконок и визуальных элементов
            registry.RegisterSingleton<IIconSourceRegistry, IconSourceRegistry>();
            registry.RegisterSingleton<IIconRenderService, IconRenderService>();
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

        }
    }
}

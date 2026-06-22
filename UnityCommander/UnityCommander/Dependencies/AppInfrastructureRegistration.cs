
using Prism.Ioc;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Abstractions.Sidebar;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Helper;
using UnityCommander.Common.Layout;
using UnityCommander.Common.Sidebar;
using UnityCommander.Core.Keyboad;
using UnityCommander.Core.Registrar;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.ToolBar.Builder;
using UnityCommander.Rendering.Icons;
using UnityCommander.Ribbon.Services;
using UnityCommander.Services;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Command;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Settings;
using UnityCommander.Services.Interfaces.Sidebar;
using UnityCommander.Services.Layout;
using UnityCommander.Services.Settings;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Dependencies
{
    public static class AppInfrastructureRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
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
            registry.RegisterSingleton<IAppConfigService, AppConfigService>();
            registry.RegisterSingleton<ISettingsProviderService, SettingsProviderService>();

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

            // Менеджер лейаута оболочки: отвечает за общую структуру интерфейса, области и их наполнение
            registry.RegisterSingleton<IShellLayoutManager, ShellLayoutManager>();

            registry.RegisterSingleton<IServiceScopeResolver, ServiceScopeResolver>();
            registry.RegisterSingleton<IServiceOverrideRegistry, ServiceOverrideRegistry>();
            registry.RegisterSingleton<ServiceOverrideResolver>();

            // Ribbon
            registry.RegisterSingleton<IRibbonModelFactory, RibbonModelFactory>();
            registry.RegisterSingleton<IRibbonRegistry, RibbonRegistry>();

            registry.RegisterSingleton<IShortcutContextService, ShortcutContextService>();
            registry.RegisterSingleton<IShortcutResolver, ShortcutResolver>();
            registry.RegisterSingleton<IShortcutRegistry, ShortcutRegistry>();
            registry.RegisterSingleton<IInputService, InputService>();

            // Старт приложения: точка инициализации всей системы при запуске
            registry.RegisterSingleton<AppInitializer>();
        }
    }
}

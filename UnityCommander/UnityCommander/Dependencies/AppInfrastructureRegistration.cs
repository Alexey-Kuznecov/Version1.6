
using Prism.Ioc;
using Prism.Services.Dialogs;

using UnityCommander.Common.Docking;
using UnityCommander.Common.Layout;
using UnityCommander.Core;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Ribbon.Core.Services;
using UnityCommander.Services;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using UnityCommander.Services.Interfaces.Settings;
using UnityCommander.Services.Settings;

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

            // Ресурсы интерфейса: поставка иконок и визуальных элементов
            registry.RegisterSingleton<IIconProviderService, PackIconProvider>();

            // Старт приложения: точка инициализации всей системы при запуске
            registry.RegisterSingleton<AppInitializer>();
        }
    }
}

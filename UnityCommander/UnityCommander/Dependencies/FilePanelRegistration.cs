
using Prism.Ioc;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Common.Selection;
using UnityCommander.Core;
using UnityCommander.Core.Behaviors.Selection;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services;
using UnityCommander.Services.Background;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Selection;
using UnityCommander.Settings;

namespace UnityCommander.Dependencies
{
    public static class FilePanelRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Сервис провайдер данных о файловой системе
            registry.RegisterSingleton<IDataProviderService, DataProviderService>();

            // Служба для обновления панелей после копирования файлов\папок
            registry.RegisterSingleton<IDirectoryChangeNotifier, DirectoryChangeNotifier>();

            // Навигационный контекст, нужен один на всё приложение
            registry.RegisterSingleton<NavigationContextDirectory>();
            registry.RegisterSingleton<NavigationManager>();

            //// Службы для управления выделением в файловых панелях
            registry.RegisterSingleton<ISelectionStrategy, SingleClickSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, ShiftSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, CtrlSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, ExtensionSelectionRuleStrategy>();
            registry.RegisterSingleton<ISelectionService, SelectionService>();
            registry.Register<ISelectionManager, SelectionManager>();

            //// Колонки по умолчанию для файлового менеджера
            registry.Register<IColumnSettingsStore, InMemoryColumnSettingsStore>(); // глобально
            registry.RegisterSingleton<IColumnProvider, DefaultColumnProvider>();
            registry.Register<IColumnStateManager, ColumnStateManager>(); // по панели
            registry.RegisterSingleton<IColumnRegistry, ColumnRegistry>(); // зависит от задач

            /// Background Services
            registry.RegisterSingleton<NodeContextRegistry>();
            registry.RegisterSingleton<ViewportMapper>();
            registry.RegisterSingleton<IBackgroundService, ColumnRefreshService>();
        }
    }
}

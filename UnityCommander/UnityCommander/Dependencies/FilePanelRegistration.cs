
using Prism.Ioc;
using UnityCommander.Common.Selection;
using UnityCommander.Core;
using UnityCommander.Core.Behaviors.Selection;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Operation;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Settings;
using UnityCommander.Services.Selection;
using UnityCommander.Services.Settings;

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

            // Калькуляторы и контроллеры для копирования файлов
            registry.RegisterSingleton<CopyProgressCalculator>();
            registry.RegisterSingleton<CopyReportCollector>();
            registry.RegisterSingleton<CopyConflictResolver>();
            registry.RegisterSingleton<CopyOperationController>();

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
            registry.Register<ColumnRegistry>(); // зависит от задач
        }
    }
}

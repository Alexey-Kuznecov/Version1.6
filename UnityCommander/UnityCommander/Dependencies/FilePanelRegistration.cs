
using Prism.Ioc;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Common.Models;
using UnityCommander.Common.Panels;
using UnityCommander.Common.Selection;
using UnityCommander.Core;
using UnityCommander.Core.Background;
using UnityCommander.Core.Behaviors.Selection;
using UnityCommander.Core.Navigation;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers.DnD;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Selection;
using UnityCommander.Settings;
using UnityCommander.SystemMetrics.Monitoring;
using UnityCommander.WPF.DragDrop;

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
            registry.RegisterSingleton<IDirectoryWatchManager, DirectoryWatchManager>();
            registry.RegisterSingleton<IDirectoryPanelUpdater, DirectoryPanelUpdater>();
            registry.RegisterSingleton<FileModelFactory>();
            registry.RegisterSingleton<FolderModelFactory>();

            // Навигационный контекст, нужен один на всё приложение
            registry.RegisterSingleton<NavigationContextDirectory>();
            registry.RegisterSingleton<NavigationManager>();

            //// Службы для управления выделением в файловых панелях
            registry.RegisterSingleton<ISelectionStrategy, ReplaceSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, RangeSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, ToggleSelectionStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, ExtensionSelectionRuleStrategy>();
            registry.RegisterSingleton<ISelectionStrategy, ContextMenuClickStrategy>();
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

            registry.RegisterSingleton<IFileStateService, FileRuntimeService>();
            registry.RegisterSingleton<IVisibleTabResolver, VisibleTabResolver>();
        }
    }
}

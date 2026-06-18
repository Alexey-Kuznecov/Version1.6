
using Prism.Ioc;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Override.Engine;
using UnityCommander.Operation;

namespace UnityCommander.Dependencies
{
    public static class CopyModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Калькуляторы и контроллеры для копирования файлов
            registry.RegisterSingleton<CopyProgressCalculator>();
            registry.RegisterSingleton<CopyReportCollector>();
            registry.RegisterSingleton<CopyConflictResolver>();
            registry.RegisterSingleton<CopyOperationController>();

            registry.RegisterSingleton<IFileOperationService, DefaultFileOperationService>();
            registry.RegisterSingleton<IFileCopyEngine, DefaultFileCopyEngine>();
        }
    }
}

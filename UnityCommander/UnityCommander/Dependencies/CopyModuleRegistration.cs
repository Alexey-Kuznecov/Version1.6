
using Prism.Ioc;
using UnityCommander.Abstractions.IO;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Common.Override.Engine;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Modules.StatusBar;
using UnityCommander.Operation;

namespace UnityCommander.Dependencies
{
    public static class CopyModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Калькуляторы и контроллеры для копирования файлов
            registry.RegisterSingleton<CopyManager>();
            registry.RegisterSingleton<CopyProgressCalculator>();
            registry.RegisterSingleton<CopyReportCollector>();
            registry.RegisterSingleton<CopyConflictResolver>();
            registry.RegisterSingleton<CopyOperationController>();

            registry.RegisterSingleton<IFileOperationService, DefaultFileOperationService>();
            registry.RegisterSingleton<IOperationIndex, OperationIndex>();
            registry.RegisterSingleton<IFileCopyEngine, DefaultFileCopyEngine>();
            registry.RegisterSingleton<ICopyOperationService, CopyOperationService>();
            registry.RegisterSingleton<IOperationProgressService, OperationProgressService>();

            registry.RegisterSingleton<IMoveStrategy, SameVolumeMoveStrategy>();
            registry.RegisterSingleton<IMoveStrategy, CrossVolumeMoveStrategy>();
            registry.RegisterSingleton<MoveStrategyResolver>();
        }
    }
}

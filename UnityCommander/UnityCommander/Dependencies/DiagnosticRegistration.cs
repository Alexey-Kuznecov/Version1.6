
using Prism.Ioc;
using UnityCommander.Commands.Diagnostic;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Performance;
using UnityCommander.Commands.Rendering;
using UnityCommander.Commands.Services;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Core.Diagnostics;

namespace UnityCommander.Dependencies
{
    public static class DiagnosticRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // Сервисы, используемые командами внутренней консоли
            registry.RegisterSingleton<ISysStatService, SysStatService>();
            registry.RegisterSingleton<IProcessOpenFilesService, ProcessOpenFilesService>();

            // Вспомогательные компоненты инфраструктуры команд
            registry.RegisterSingleton<ICommandArgumentParser, CommandArgumentParser>();
            registry.RegisterSingleton<IConsoleRenderer<SystemStats>, SystemStatsRenderer>();

            registry.RegisterSingleton<IDiagnosticRender, DiagnosticRender>();
            registry.RegisterSingleton<IDiagnosticPipeline, DiagnosticPipeline>();
            registry.RegisterSingleton<IDiagnosticRegistry, DiagnosticRegistry>();
        }
    }
}

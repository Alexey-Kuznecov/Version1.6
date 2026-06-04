
using CommandSystem.Abstractions;
using Prism.Ioc;
using Prism.Modularity;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Commands;
using UnityCommander.Modules.FilePanel.States.Resolver;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel
{
    [ModuleDependency(nameof(FilePanelModule))]
    public class FilePanelCommandModule : IModule
    {
        private readonly IContainerProvider _container;

        public FilePanelCommandModule(IContainerProvider container)
        {
            _container = container;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Здесь можно зарегистрировать зависимости модуля, если нужно
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var commandService = containerProvider.Resolve<CommandService>();
            var filePanelProvider = containerProvider.Resolve<FilePanelCommandProvider>();
            var presentation = containerProvider.Resolve<CommandPresentationProvider>();


            commandService.Register(CommandFactory.Create(
                CommandNames.Panel.GetCurrentPath,
                filePanelProvider.GetCurrentPath
            ));

            commandService.Register(CommandFactory.Create(
                CommandNames.Panel.SetCurrentPath,
                filePanelProvider.SetCurrentPath
            ));

            commandService.RegisterUndoable(
                CommandFactory.Create(CommandNames.File.Delete, ExecuteDeleteAsync,
                contextTypes: typeof(FilePanelContextMenu)));
        }

        private Task<UndoToken> ExecuteDeleteAsync(CommandContext ctx)
        {
            var path = ctx.Parameter?.ToString();

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return Task.FromResult<UndoToken>(null);

            var backup = Path.GetTempFileName();
            File.Copy(path, backup);
            File.Delete(path);

            return Task.FromResult<UndoToken>(new DelegateUndoToken(
                undo: () => UndoDeleteAsync(backup, path),
                redo: () => RedoDeleteAsync(path)
            ));
        }

        private Task UndoDeleteAsync(string backup, string path)
        {
            File.Copy(backup, path, overwrite: true);
            return Task.CompletedTask;
        }

        private Task RedoDeleteAsync(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }
    }
}

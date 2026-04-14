using CommandSystem.Abstractions;
using Prism.Ioc;
using Prism.Modularity;
using System.IO;
using System.Threading.Tasks;
using UnityCommander.Services;

namespace UnityCommander.Modules.FilePanel
{
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

            commandService.Register(
                new CommandMetadata("getcurpath", "Получает текущий путь директории"),
                filePanelProvider.GetCurrentPathCommand);

            commandService.Register(
                new CommandMetadata("setcurpath", "Устанавливает текущий путь директории"),
                filePanelProvider.SetCurrentPathCommand);

            commandService.RegisterUndoable(
                new CommandMetadata("file.delete", "Удалить файл/Восстановить файл"),
                async ctx =>
                {
                    var path = ctx.Parameter?.ToString();

                    if (string.IsNullOrEmpty(path) || !File.Exists(path))
                        return null;

                    var backup = Path.GetTempFileName();
                    File.Copy(path, backup);
                    File.Delete(path);

                    return new DelegateUndoToken(
                        undo: async () =>
                        {
                            File.Copy(backup, path, overwrite: true);
                        },
                        redo: async () =>
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                        }
                    );
                });
        }
    }
}

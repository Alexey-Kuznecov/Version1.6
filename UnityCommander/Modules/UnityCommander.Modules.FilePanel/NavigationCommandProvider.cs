
using CommandSystem.Abstractions;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;
using UnityCommander.Core.Navigation;

namespace UnityCommander.Modules.FilePanel
{
    public class NavigationCommandProvider
    {
        private readonly NavigationManager _navigationService;

        public NavigationCommandProvider(
            NavigationManager navigationService)
        {
            _navigationService = navigationService;
        }

        public Task Navigate(CommandContext ctx)
        {
            var path = ctx.Parameter?.ToString();

            if (string.IsNullOrWhiteSpace(path))
                return Task.CompletedTask;

            _navigationService.TryNavigateTo(path);

            return Task.CompletedTask;
        }

        public Task Refresh(CommandContext ctx)
        {
            if (ctx.Parameter is CommandArguments args)
            {
                var path = args.Get<string>(0);
                var force = args.Get<bool>(1);

                _navigationService.TryNavigateTo(path, force);
            }

            return Task.CompletedTask;
        }
    }
}

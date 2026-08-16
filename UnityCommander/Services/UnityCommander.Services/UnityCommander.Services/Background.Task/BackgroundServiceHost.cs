
using System.Collections.Generic;
using System.Threading;
using UnityCommander.Abstractions.Background;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Services.Background
{
    public sealed class BackgroundServiceHost
    {
        private readonly IBackgroundServiceRegistry _registry;

        public BackgroundServiceHost(IBackgroundServiceRegistry registry)
        {
            _registry = registry;
        }

        public void Start(CancellationToken token)
        {
            foreach (var service in _registry.GetAll())
            {
                if (service.AutoStart)
                    _ = service.RunAsync(token);
            }
        }

        public IEnumerable<IStatusBarItem> GetItems()
        {
            foreach (var service in _registry.GetAll())
            {
                if (service is IStatusBarProvider provider)
                {
                    foreach (var item in provider.GetItems())
                        yield return item;
                }
            }
        }
    }
}

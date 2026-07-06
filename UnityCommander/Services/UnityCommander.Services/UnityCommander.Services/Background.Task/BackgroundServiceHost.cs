
using System.Collections.Generic;
using System.Threading;
using UnityCommander.Abstractions.Background;

namespace UnityCommander.Services.Background
{
    public sealed class BackgroundServiceHost
    {
        private readonly IEnumerable<IBackgroundService> _services;

        public BackgroundServiceHost(IEnumerable<IBackgroundService> services)
        {
            _services = services;
        }

        public void Start(CancellationToken token)
        {
            foreach (var service in _services)
                _ = service.RunAsync(token);
        }
    }
}

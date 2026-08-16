
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Background;

namespace UnityCommander.Core.Background
{
    public class BackgroundServiceRegistry : IBackgroundServiceRegistry
    {
        private readonly Dictionary<string, IBackgroundService> _services = new();

        public event Action<string>? OwnerUnload;

        public BackgroundServiceRegistry(IEnumerable<IBackgroundService> services)
        {
            foreach (var service in services)
            {
                if (_services.ContainsKey(service.Id))
                    throw new InvalidOperationException(
                        $"Background service '{service.Id}' is already registered.");

                _services.Add(service.Id, service);
            }
        }

        public void Register(IBackgroundService service)
        {
            if (_services.ContainsKey(service.Id))
                throw new InvalidOperationException(
                    $"Background service '{service.Id}' is already registered.");

            _services[service.Id] = service;
        }

        public IBackgroundService? Get(string id)
        {
            _services.TryGetValue(id, out var service);
            return service;
        }

        public IEnumerable<IBackgroundService> GetAll()
        {
            return _services.Values;
        }

        public void Unregister(string id)
        {
            _services.Remove(id);
        }

        public void Cleanup(string ownerId)
        {
            var services = _services.Values
                .Where(x => x.OwnerId == ownerId)
                .ToList();

            foreach (var service in services)
                _services.Remove(service.Id);

            OwnerUnload?.Invoke(ownerId);
        }
    }
}

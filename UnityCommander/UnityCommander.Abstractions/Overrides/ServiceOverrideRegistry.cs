
namespace UnityCommander.Abstractions.Overrides
{
    public sealed class ServiceOverrideRegistry
     : IServiceOverrideRegistry
    {
        private readonly Dictionary<Type, ServiceOverrideEntry> _entries =
            new();

        public void Register(
            string ownerId,
            Type serviceType,
            Type implementationType)
        {
            _entries[serviceType] =
                new ServiceOverrideEntry
                {
                    OwnerId = ownerId,
                    ServiceType = serviceType,
                    ImplementationType = implementationType
                };
        }

        public bool TryGet(
            Type serviceType,
            out ServiceOverrideEntry entry)
        {
            return _entries.TryGetValue(
                serviceType,
                out entry);
        }

        public void Cleanup(string ownerId)
        {
            var keys =
                _entries
                    .Where(x => x.Value.OwnerId == ownerId)
                    .Select(x => x.Key)
                    .ToList();

            foreach (var key in keys)
            {
                _entries.Remove(key);
            }
        }

        public void Register<TService, TImplementation>(
            string ownerId)
            where TImplementation : TService
        {
            Register(
                ownerId,
                typeof(TService),
                typeof(TImplementation));
        }
    }
}

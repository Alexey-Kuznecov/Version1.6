using UnityCommander.Testing.Abstarctions;

namespace UnityCommander.Testing.Infrastructure
{
    public sealed class TestServiceCatalog : ITestServiceCatalog
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, Func<ITestServiceCatalog, object>> _factories = new();

        public void RegisterInstance<TService>(TService instance)
        {
            _instances[typeof(TService)] = instance!;
        }

        public void RegisterSingleton<TService>(
            Func<ITestServiceCatalog, TService> factory)
        {
            _factories[typeof(TService)] = c => factory(c)!;
        }

        public TService Resolve<TService>()
        {
            var type = typeof(TService);

            if (_instances.TryGetValue(type, out var existing))
                return (TService)existing;

            if (_factories.TryGetValue(type, out var factory))
            {
                var created = factory(this);
                _instances[type] = created;
                return (TService)created;
            }

            throw new InvalidOperationException(
                $"Service {type.Name} is not registered");
        }
    }
}

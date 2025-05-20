using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Plugins
{
    internal class PluginContext : IPluginContext
    {
        private readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance;
        }

        public T? GetService<T>() where T : class
        {
            _services.TryGetValue(typeof(T), out var service);
            return service as T;
        }
    }
}

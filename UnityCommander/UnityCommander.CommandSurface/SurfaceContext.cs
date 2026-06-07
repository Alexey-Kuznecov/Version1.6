
namespace UnityCommander.CommandSurface
{
    public class SurfaceContext
    {
        private readonly Dictionary<Type, object> _data = new();

        public void Set<T>(T value)
            => _data[typeof(T)] = value!;

        public bool TryGet<T>(out T value)
        {
            if (_data.TryGetValue(typeof(T), out var obj))
            {
                value = (T)obj;
                return true;
            }

            value = default!;
            return false;
        }

        public T? Get<T>()
            => TryGet<T>(out var value) ? value : default;

        public bool Has(Type type) => _data.ContainsKey(type);
    }
}

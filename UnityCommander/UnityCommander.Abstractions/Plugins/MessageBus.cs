
namespace UnityCommander.Abstractions.Plugins
{
    public sealed class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        private readonly object _lock = new();

        public IDisposable Subscribe<T>(Func<T, ValueTask> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(T), out var handlers))
                {
                    handlers = new List<object>();
                    _handlers.Add(typeof(T), handlers);
                }

                handlers.Add(handler);
            }

            return new Subscription<T>(this, handler);
        }

        public async ValueTask PublishAsync<T>(T message)
        {
            List<Func<T, ValueTask>> handlers;

            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(T), out var list))
                    return;

                handlers = list.Cast<Func<T, ValueTask>>().ToList();
            }

            foreach (var handler in handlers)
            {
                await handler(message);
            }
        }

        private void Unsubscribe<T>(Func<T, ValueTask> handler)
        {
            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(T), out var handlers))
                    return;

                handlers.Remove(handler);

                if (handlers.Count == 0)
                    _handlers.Remove(typeof(T));
            }
        }

        private sealed class Subscription<T> : IDisposable
        {
            private readonly MessageBus _bus;
            private readonly Func<T, ValueTask> _handler;
            private bool _disposed;

            public Subscription(
                MessageBus bus,
                Func<T, ValueTask> handler)
            {
                _bus = bus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _bus.Unsubscribe(_handler);
            }
        }
    }
}

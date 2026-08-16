
using System;
using System.Collections.Concurrent;
using UnityCommander.Abstractions;

namespace UnityCommander.Common.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, Delegate?> _handlers = new();

        public void Subscribe<T>(EventHandler<T> handler)
            where T : EventArgs
        {
            _handlers.AddOrUpdate(
                typeof(T),
                handler,
                (_, existing) => Delegate.Combine(existing, handler));
        }

        public void Unsubscribe<T>(EventHandler<T> handler)
            where T : EventArgs
        {
            _handlers.AddOrUpdate(
                typeof(T),
                null,
                (_, existing) => Delegate.Remove(existing, handler));
        }

        public void Publish<T>(object? sender, T args)
            where T : EventArgs
        {
            if (_handlers.TryGetValue(typeof(T), out var del))
            {
                foreach (EventHandler<T> handler in del.GetInvocationList())
                {
                    handler(sender, args);
                }
            }
        }
    }
}

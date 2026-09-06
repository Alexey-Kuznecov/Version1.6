
namespace UnityCommander.Abstractions
{
    public interface IEventBus
    {
        void Publish<T>(object? sender, T args)
            where T : EventArgs;

        void Subscribe<T>(EventHandler<T> handler)
            where T : EventArgs;

        void Unsubscribe<T>(EventHandler<T> handler)
            where T : EventArgs;
    }
}

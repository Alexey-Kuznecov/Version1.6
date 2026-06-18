
namespace UnityCommander.Abstractions.Plugins
{
    public interface IMessageBus
    {
        ValueTask PublishAsync<T>(T message);
        
        IDisposable Subscribe<T>(Func<T, ValueTask> handler);
    }
}

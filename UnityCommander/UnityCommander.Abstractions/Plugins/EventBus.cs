
namespace UnityCommander.Core.Plugin
{
    public class EventBus : IEventBus
    {
        public void Publish<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }
    }
}

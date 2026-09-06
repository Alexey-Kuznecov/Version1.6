
namespace UnityCommander.Abstractions.Panels
{
    public interface ITabStateRegistry
    {
        public void Register(TabState state);

        public void Unregister(Guid tabId);

        public TabState? Get(Guid tabId);
    }
}

namespace UnityCommander.Abstractions.Background
{
    public interface IBackgroundServiceRegistry : IOwnedRegistry
    {
        event Action<string>? OwnerUnload;

        void Register(IBackgroundService service);

        void Unregister(string id);

        IBackgroundService? Get(string id);

        IEnumerable<IBackgroundService> GetAll();
    }
}

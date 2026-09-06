
namespace UnityCommander.Abstractions.Overrides
{
    public interface IServiceOverrideRegistry : IOwnedRegistry
    {
        void Register(
            string ownerId,
            Type serviceType,
            Type implementationType);

        public void Register<TService, TImplementation>(
        string ownerId)
        where TImplementation : TService;

        bool TryGet(
            Type serviceType,
            out ServiceOverrideEntry entry);
    }
}

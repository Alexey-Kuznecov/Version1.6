
namespace UnityCommander.Abstractions.Ribbon
{
    public interface IRibbonBindingRegistry : IOwnedRegistry
    {
        void Register(RibbonBinding binding);

        IReadOnlyCollection<RibbonBinding> GetAll();
    }
}

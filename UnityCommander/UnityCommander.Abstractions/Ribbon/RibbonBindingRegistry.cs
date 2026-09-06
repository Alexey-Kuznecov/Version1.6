
using System.Windows.Input;

namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonBindingRegistry : IRibbonBindingRegistry
    {
        private readonly List<RibbonBinding> _bindings = new();

        public void Register(RibbonBinding binding)
        {
            _bindings.Add(binding);
        }

        public IReadOnlyCollection<RibbonBinding> GetAll()
        {
            return _bindings;
        }

        public void Cleanup(string ownerId)
        {
        }
    }
}


namespace UnityCommander.Abstractions.Ribbon
{
    public sealed class RibbonBindingBuilder
    {
        private readonly List<RibbonBinding> _bindings;

        public RibbonBindingBuilder(
            List<RibbonBinding> bindings)
        {
            _bindings = bindings;
        }

        public RibbonBindingBuilder Bind(
            string commandId,
            string tabId,
            string groupId,
            int order = 0)
        {
            _bindings.Add(
                new RibbonBinding
                {
                    CommandId = commandId,
                    TabId = tabId,
                    GroupId = groupId,
                    Order = order
                });

            return this;
        }
    }
}

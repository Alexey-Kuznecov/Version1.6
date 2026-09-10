
namespace UnityCommander.UI.Interaction
{
    public sealed class ContextActionService : IContextActionService
    {
        private readonly IEnumerable<IContextActionProvider> _providers;

        public ContextActionService(
            IEnumerable<IContextActionProvider> providers)
        {
            _providers = providers;
        }

        public IReadOnlyList<IContextAction> GetActions(
            IInteractionContext context)
        {
            return _providers
                .SelectMany(x => x.GetActions(context))
                .Where(x => x.CanExecute(context))
                .ToList();
        }
    }
}

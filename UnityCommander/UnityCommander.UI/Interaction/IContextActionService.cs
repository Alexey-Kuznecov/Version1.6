
namespace UnityCommander.UI.Interaction
{
    public interface IContextActionService
    {
        IReadOnlyList<IContextAction> GetActions(
            IInteractionContext context);
    }
}

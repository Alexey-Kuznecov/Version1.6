
namespace UnityCommander.UI.Interaction
{
    public interface IContextActionProvider
    {
        IEnumerable<IContextAction>? GetActions(
            IInteractionContext context);
    }
}

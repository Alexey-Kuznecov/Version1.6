
namespace UnityCommander.Common.Selection
{
    public interface ISelectionStrategy
    {
        SelectionActionType ActionType { get; }
        void Select(ISelectionContext context, SelectionAction action);
    }
}

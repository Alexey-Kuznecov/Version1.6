
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class ReplaceSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.SingleClick;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            foreach (var item in ctx.Items)
                item.IsSelected = false;

            ctx.Items[action.TargetIndex].IsSelected = true;

            ctx.FocusedIndex = action.TargetIndex;
            ctx.AnchorIndex = action.TargetIndex;
        }
    }
}

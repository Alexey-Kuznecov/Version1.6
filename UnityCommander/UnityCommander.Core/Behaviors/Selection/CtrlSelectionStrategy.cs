
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class CtrlSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.CtrlClick;

        public void Select(ISelectionContext ctx, SelectionAction action)
        {
            var item = ctx.Items[action.TargetIndex];

            item.IsSelected = !item.IsSelected;

            ctx.FocusedIndex = action.TargetIndex;
        }
    }
}

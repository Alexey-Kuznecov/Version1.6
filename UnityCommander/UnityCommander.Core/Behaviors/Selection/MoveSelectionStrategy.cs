
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public sealed class MoveSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.Move;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            if (ctx.Items.Count == 0)
                return;

            var current = ctx.FocusedIndex;

            if (current < 0)
                current = 0;

            var next = current + action.Offset;

            if (next < 0)
                next = ctx.Items.Count - 1;
            else if (next >= ctx.Items.Count)
                next = 0;

            foreach (var item in ctx.Items)
                item.IsSelected = false;

            ctx.Items[next].IsSelected = true;

            ctx.FocusedIndex = next;
            ctx.AnchorIndex = next;
        }
    }
}

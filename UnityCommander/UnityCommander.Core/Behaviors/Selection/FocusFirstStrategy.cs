
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public sealed class FocusFirstStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.FocusFirst;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            foreach (var item in ctx.Items)
                item.IsSelected = false;

            if (ctx.Items.Count == 0)
                return;

            ctx.FocusedIndex = 0;
            ctx.AnchorIndex = 0;
            ctx.Items[0].IsSelected = true;
        }
    }
}

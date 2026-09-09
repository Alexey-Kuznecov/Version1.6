
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public sealed class SelectAllStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.SelectAll;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            foreach (var item in ctx.Items)
                item.IsSelected = true;

            if (ctx.Items.Count > 0)
            {
                ctx.FocusedIndex = 0;
                ctx.AnchorIndex = 0;
            }
        }
    }
}

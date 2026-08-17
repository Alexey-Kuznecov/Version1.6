
using System;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class RangeSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.ShiftClick;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            if (ctx.AnchorIndex < 0)
                ctx.AnchorIndex = action.TargetIndex;

            int start = Math.Min(
                ctx.AnchorIndex,
                action.TargetIndex);

            int end = Math.Max(
                ctx.AnchorIndex,
                action.TargetIndex);

            foreach (var item in ctx.Items)
                item.IsSelected = false;

            for (int i = start; i <= end; i++)
                ctx.Items[i].IsSelected = true;

            ctx.FocusedIndex = action.TargetIndex;
        }
    }
}

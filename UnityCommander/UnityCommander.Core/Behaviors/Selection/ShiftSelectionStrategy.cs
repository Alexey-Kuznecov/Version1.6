
using System;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class ShiftSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.ShiftClick;

        public void Select(ISelectionContext ctx, SelectionAction action)
        {
            if (ctx.FocusedIndex < 0)
            {
                ctx.FocusedIndex = action.TargetIndex;
            }

            int start = Math.Min(ctx.FocusedIndex, action.TargetIndex);
            int end = Math.Max(ctx.FocusedIndex, action.TargetIndex);

            foreach (var item in ctx.Items)
                item.IsSelected = false;

            for (int i = start; i <= end; i++)
                ctx.Items[i].IsSelected = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class ShiftSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.ShiftClick;

        public void Select(ISelectionContext context, SelectionAction action)
        {
            var start = Math.Min(context.FocusedIndex, action.TargetIndex);
            var end = Math.Max(context.FocusedIndex, action.TargetIndex);

            for (int i = start; i <= end; i++)
                context.Items[i].IsSelected = true;
        }
    }
}

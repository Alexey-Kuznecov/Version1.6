using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class SingleClickSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.SingleClick;

        public void Select(ISelectionContext ctx, SelectionAction action)
        {
            // Снимаем выделение всех
            foreach (var it in ctx.Items)
                it.IsSelected = false;

            // Выделяем один
            ctx.Items[action.TargetIndex].IsSelected = true;
            ctx.FocusedIndex = action.TargetIndex;
        }
    }
}

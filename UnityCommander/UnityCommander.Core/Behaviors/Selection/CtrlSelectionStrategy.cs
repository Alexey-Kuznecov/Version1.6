using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class CtrlSelectionStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.CtrlClick;

        public void Select(ISelectionContext context, SelectionAction action)
        {
            var item = context.Items[action.TargetIndex];
            item.IsSelected = !item.IsSelected;
        }
    }
}

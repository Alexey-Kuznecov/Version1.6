
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public sealed class ContextMenuClickStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType =>
            SelectionActionType.ContextMenuClick;

        public void Select(
            ISelectionContext ctx,
            SelectionAction action)
        {
            var item = ctx.Items[action.TargetIndex];

            // Уже находится в выделении —
            // ничего не меняем.
            if (item.IsSelected)
            {
                ctx.FocusedIndex = action.TargetIndex;
                return;
            }

            // Иначе делаем его единственным выделенным.
            foreach (var it in ctx.Items)
                it.IsSelected = false;

            item.IsSelected = true;
            ctx.FocusedIndex = action.TargetIndex;
        }
    }
}

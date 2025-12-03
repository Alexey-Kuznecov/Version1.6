using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Core.Behaviors.Selection
{
    public class ExtensionSelectionRuleStrategy : ISelectionStrategy
    {
        public SelectionActionType ActionType => SelectionActionType.SelectByExtension;

        public void Select(ISelectionContext context, SelectionAction action)
        {
            if (context?.Items == null || string.IsNullOrEmpty(action.Parameter))
                return;

            string ext = action.Parameter.ToLowerInvariant();

            foreach (var item in context.Items)
            {
                if (!string.IsNullOrEmpty(item.Key) && item.Key.ToLowerInvariant().EndsWith(ext))
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}

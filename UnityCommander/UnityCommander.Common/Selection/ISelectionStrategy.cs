using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Common.Selection
{
    public interface ISelectionStrategy
    {
        SelectionActionType ActionType { get; }
        void Select(ISelectionContext context, SelectionAction action);
    }
}

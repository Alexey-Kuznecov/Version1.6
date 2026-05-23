using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Selection;

namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionManager
    {
        event Action SelectionChanged;

        IReadOnlyCollection<ISelectableItem> SelectedItems { get; }
        void Handle(ISelectionContext ctx, SelectionAction action);
    }
}

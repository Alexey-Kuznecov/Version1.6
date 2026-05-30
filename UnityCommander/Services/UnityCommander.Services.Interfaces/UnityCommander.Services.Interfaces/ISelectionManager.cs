using System;
using System.Collections.Generic;
using UnityCommander.Common.Selection;

namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionManager
    {
        event Action SelectionChanged;

        public ISelectableItem FocusedItem { get; set; }

        IReadOnlyCollection<ISelectableItem> SelectedItems { get; }
        void Handle(ISelectionContext ctx, SelectionAction action);
    }
}

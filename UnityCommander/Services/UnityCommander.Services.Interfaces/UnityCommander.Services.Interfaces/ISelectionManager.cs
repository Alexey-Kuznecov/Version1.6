using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Selection;

namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionManager
    {
        event Action SelectionChanged;

        public ISelectableItem FocusedItem { get; set; }

        IReadOnlyCollection<ISelectableItem> SelectedItems { get; }
        
        void Handle(SelectionAction action);

        void ResetContext(IEnumerable<ISelectableItem> items);

        void SetItems(IEnumerable<ISelectableItem> items);

        void ClearSelection();
    }
}

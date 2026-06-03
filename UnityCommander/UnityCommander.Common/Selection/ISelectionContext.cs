
using System.Collections.Generic;

namespace UnityCommander.Common.Selection
{
    public interface ISelectionContext
    {
        IReadOnlyList<ISelectableItem> Items { get; }

        int FocusedIndex { get; set; }

        // методы доступа/модификации выделения внутри контекста
        void ClearSelection();
        void AddToSelection(IEnumerable<ISelectableItem> items);
    }
}

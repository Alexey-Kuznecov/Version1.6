
using System.Collections.Generic;
using UnityCommander.Abstractions.Selection;

namespace UnityCommander.Common.Selection
{
    public interface ISelectionContext
    {
        IReadOnlyList<ISelectableItem> Items { get; }

        int FocusedIndex { get; set; }
        int AnchorIndex { get; set; }

        void SetItems(IEnumerable<ISelectableItem> items);

        void Reset();
    }
}

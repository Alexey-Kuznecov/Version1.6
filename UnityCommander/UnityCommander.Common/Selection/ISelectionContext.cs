using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;

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

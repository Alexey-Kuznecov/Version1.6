using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Common.Selection
{
    public class SelectionContext : BindableBase, ISelectionContext
    {
        private int _focusedIndex = -1;
        
        private readonly List<ISelectableItem> _selected = new();
        public int FocusedIndex
        {
            get => _focusedIndex;
            set => SetProperty(ref _focusedIndex, value);
        }

        public IReadOnlyList<ISelectableItem> Items { get; }

        public SelectionContext(IEnumerable<ISelectableItem> items)
        {
            Items = items.ToList();
        }


        public void ClearSelection()
        {
            foreach (var item in _selected)
                item.IsSelected = false;
            _selected.Clear();
        }

        public void AddToSelection(IEnumerable<ISelectableItem> items)
        {
            foreach (var item in items)
            {
                if (!_selected.Contains(item))
                {
                    item.IsSelected = true;
                    _selected.Add(item);
                }
            }
        }
    }
}

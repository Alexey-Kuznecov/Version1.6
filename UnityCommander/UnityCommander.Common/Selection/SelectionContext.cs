
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityCommander.Abstractions.Selection;

namespace UnityCommander.Common.Selection
{
    public class SelectionContext : BindableBase, ISelectionContext
    {
        private IReadOnlyList<ISelectableItem> _items =
            Array.Empty<ISelectableItem>();

        private int _focusedIndex = -1;
        private int _anchorIndex = -1;

        public SelectionContext()
        {
            //Debug.WriteLine(
            //    $"[Selection] Context CREATED: {GetHashCode()}, Items={Items.Count}");
        }

        public IReadOnlyList<ISelectableItem> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        public int FocusedIndex
        {
            get => _focusedIndex;
            set => SetProperty(ref _focusedIndex, value);
        }

        public int AnchorIndex
        {
            get => _anchorIndex;
            set => SetProperty(ref _anchorIndex, value);
        }

        public void SetItems(IEnumerable<ISelectableItem> items)
        {
            Items = items.ToList();

            //Debug.WriteLine(
            //     $"[Selection] SetItems: Count={Items.Count}, " +
            //     $"Manager={GetHashCode()}");
        }

        public void Reset()
        {
            Items = Array.Empty<ISelectableItem>();
            FocusedIndex = -1;
            AnchorIndex = -1;
        }
    }
}

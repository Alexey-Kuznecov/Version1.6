
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionManager : ISelectionManager
    {
        private readonly Dictionary<SelectionActionType, ISelectionStrategy> strategies;
        
        private ISelectionContext _context 
            = new SelectionContext();

        public event Action SelectionChanged;

        public IReadOnlyCollection<ISelectableItem> SelectedItems =>
            _context.Items
                .Where(x => x.IsSelected)
                .ToList();

        public ISelectableItem FocusedItem { get; set; }

        public SelectionManager(
            IEnumerable<ISelectionStrategy> strategies)
        {
            this.strategies = strategies.ToDictionary(x => x.ActionType);
        }

        public void Handle(SelectionAction action)
        {
            Debug.WriteLine(
                $"[Selection] Handle: {action.Type}, " +
                $"Index={action.TargetIndex}");

            if (!strategies.TryGetValue(action.Type, out var strategy))
                return; // или лог ошибки
            strategies[action.Type].Select(_context, action);
            RaiseChanged();
        }

        public void ClearSelection()
        {
            Debug.WriteLine("[Selection] ClearSelection");

            foreach (var item in _context.Items)
                item.IsSelected = false;

            _context.FocusedIndex = -1;
            _context.AnchorIndex = -1;

            SelectionChanged?.Invoke();
        }

        public void ResetContext(IEnumerable<ISelectableItem> items)
        {
            _context.Reset();
            _context.SetItems(items);
        }

        public void SetItems(IEnumerable<ISelectableItem> items)
        {
            _context.SetItems(items);
        }

        private void RaiseChanged()
        {
            SelectionChanged?.Invoke();
        }
    }
}

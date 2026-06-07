
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionManager : ISelectionManager
    {
        private readonly Dictionary<SelectionActionType, ISelectionStrategy> strategies;
        
        private ISelectionContext _context 
            = new SelectionContext(new List<ISelectableItem>());

        public event Action SelectionChanged;

        public IReadOnlyCollection<ISelectableItem> SelectedItems
            => _context.Items;

        public ISelectableItem FocusedItem { get; set; }

        public SelectionManager(
            IEnumerable<ISelectionStrategy> strategies)
        {
            this.strategies = strategies.ToDictionary(x => x.ActionType);
        }

        public void Handle(ISelectionContext ctx, SelectionAction action)
        {
            _context = ctx;

            if (!strategies.TryGetValue(action.Type, out var strategy))
                return; // или лог ошибки
            strategies[action.Type].Select(ctx, action);
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            SelectionChanged?.Invoke();
        }

        public void Clear()
        {
            _context
             = new SelectionContext(new List<ISelectableItem>());
        }
    }
}

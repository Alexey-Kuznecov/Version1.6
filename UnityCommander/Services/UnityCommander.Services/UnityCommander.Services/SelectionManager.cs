
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Selection;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Common.Selection;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionManager : ISelectionManager, IDiagnosticReporter
    {
        private readonly Dictionary<SelectionActionType, ISelectionStrategy> strategies;

        private readonly ILogger _logger;

        private readonly object _lock = new();

        private ISelectionContext _context 
            = new SelectionContext();

        public event Action SelectionChanged;

        public IReadOnlyCollection<ISelectableItem> SelectedItems =>
            _context.Items
                .Where(x => x.IsSelected)
                .ToList();

        public ISelectableItem FocusedItem { get; set; }

        public string Name => "selection";

        public DiagnosticCardinality Cardinality
            => DiagnosticCardinality.Multiple;

        public SelectionManager(
            IEnumerable<ISelectionStrategy> strategies,
            IDiagnosticRegistry diagnostic, 
            LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<SelectionManager>(LogScope.Runtime);
      
            diagnostic.Register(this);
            this.strategies = strategies.ToDictionary(x => x.ActionType);
        }

        public void Handle(SelectionAction action)
        {

            if (!strategies.TryGetValue(action.Type, out var strategy))
            {
                _logger.Info(
                    $"[Selection] Handle SKIP: " +
                    $"No strategy for {action.Type}");

                return;
            }

            strategy.Select(_context, action);

            RaiseChanged();
        }

        public void ClearSelection()
        {
            _logger.Info("[Selection] ClearSelection");

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

        public void Report(IDiagnosticWriter writer)
        {
            if (FocusedItem != null)
            {
                writer.WriteLine(
                    $"First selected type: {FocusedItem.GetType().FullName}");
            }

            var selected = _context.Items
                .Where(x => x.IsSelected)
                .ToList();

            writer.WriteLine("=== Selection Manager ===");

            writer.WriteLine(
                $"Manager: {GetHashCode():X8}");

            writer.WriteLine(
                $"Context: {_context.GetHashCode():X8}");

            writer.WriteLine("");

            writer.WriteLine("=== Context ===");

            writer.WriteLine(
                $"Items: {_context.Items.Count}");

            writer.WriteLine(
                $"Selected flags: {selected.Count}");

            writer.WriteLine(
                $"AnchorIndex: {_context.AnchorIndex}");

            writer.WriteLine(
                $"FocusedIndex: {_context.FocusedIndex}");

            writer.WriteLine(
                $"Anchor valid: " +
                $"{_context.AnchorIndex >= 0 && _context.AnchorIndex < _context.Items.Count}");

            writer.WriteLine(
                $"Focused valid: " +
                $"{_context.FocusedIndex >= 0 && _context.FocusedIndex < _context.Items.Count}");

            writer.WriteLine("");

            writer.WriteLine("=== SelectedItems Property ===");

            var selectedItems = SelectedItems;

            writer.WriteLine(
                $"Count: {selectedItems.Count}");

            writer.WriteLine(
                $"Matches context flags: {selectedItems.Count == selected.Count}");
        }
    }
}

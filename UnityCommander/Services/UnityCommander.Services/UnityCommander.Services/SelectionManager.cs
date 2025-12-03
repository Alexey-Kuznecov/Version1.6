
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Selection;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Selection
{
    public class SelectionManager : ISelectionManager
    {
        private readonly Dictionary<SelectionActionType, ISelectionStrategy> strategies;

        public SelectionManager(IEnumerable<ISelectionStrategy> strategies)
        {
            this.strategies = strategies.ToDictionary(x => x.ActionType);
        }

        public void Handle(ISelectionContext ctx, SelectionAction action)
        {
            if (!strategies.TryGetValue(action.Type, out var strategy))
                return; // или лог ошибки
            strategies[action.Type].Select(ctx, action);
        }
    }
}

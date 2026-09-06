
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Dialog;

namespace UnityCommander.Core.Registrar
{
    public sealed class DialogRegistry : IDialogRegistry
    {
        private readonly Dictionary<string, IDialogDefinition> _dialogsById = new();

        private readonly Dictionary<Type, IDialogDefinition> _dialogsByType = new();

        public void Register(IDialogDefinition dialogDefinition)
        {
            ArgumentNullException.ThrowIfNull(dialogDefinition);

            _dialogsById.Add(
                dialogDefinition.Id,
                dialogDefinition);

            _dialogsByType.Add(
                dialogDefinition.ViewType,
                dialogDefinition);
        }

        public bool Unregister(string id)
        {
            if (!_dialogsById.TryGetValue(id, out var dialog))
                return false;

            _dialogsById.Remove(id);
            _dialogsByType.Remove(dialog.ViewType);

            return true;
        }

        public bool TryGet(
            string id,
            out IDialogDefinition dialogDefinition)
        {
            return _dialogsById.TryGetValue(
                id,
                out dialogDefinition!);
        }

        public bool TryGet<TDialog>(
            out IDialogDefinition registration)
        {
            return _dialogsByType.TryGetValue(
                typeof(TDialog),
                out registration!);
        }

        public bool TryGet(
            Type dialogType,
            out IDialogDefinition registration)
        {
            return _dialogsByType.TryGetValue(
                dialogType,
                out registration!);
        }

        public IReadOnlyCollection<IDialogDefinition> GetAll()
        {
            return _dialogsById.Values;
        }

        public void Cleanup(string pluginId)
        {
            var dialogs = _dialogsById
                .Where(x => x.Value.OwnerId == pluginId)
                .Select(x => x.Value)
                .ToList();

            foreach (var dialog in dialogs)
            {
                _dialogsById.Remove(dialog.Id);
                _dialogsByType.Remove(dialog.ViewType);
            }
        }
    }
}

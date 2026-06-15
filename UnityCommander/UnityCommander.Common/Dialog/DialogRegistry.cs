
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Common.Dialog
{
    public sealed class DialogRegistry : IDialogRegistry
    {
        private readonly Dictionary<string, IDialogDefinition> _dialogs = new();

        public void Register(IDialogDefinition dialogDefinition)
         {
            ArgumentNullException.ThrowIfNull(dialogDefinition);

            _dialogs.Add(dialogDefinition.Id, dialogDefinition);
        }

        public bool Unregister(string id)
        {
            return _dialogs.Remove(id);
        }

        public bool TryGet(string id, out IDialogDefinition dialogDefinition)
        {
            return _dialogs.TryGetValue(id, out dialogDefinition!);
        }

        public IReadOnlyCollection<IDialogDefinition> GetAll()
        {
            return _dialogs.Values;
        }

        public void Cleanup(string pluginId)
        {
            var dialogs = _dialogs
                .Where(x => x.Value.OwnerId == pluginId)
                .Select(x => x.Key)
                .ToList();

            foreach (var id in dialogs)
            {
                _dialogs.Remove(id);
            }
        }
    }
}

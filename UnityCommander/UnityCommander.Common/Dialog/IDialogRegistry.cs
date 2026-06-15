

using System.Collections.Generic;

namespace UnityCommander.Common.Dialog
{
    public interface IDialogRegistry
    {
        void Register(IDialogDefinition registration);

        bool Unregister(string id);

        bool TryGet(string id, out IDialogDefinition registration);

        IReadOnlyCollection<IDialogDefinition> GetAll();

        void Cleanup(string pluginId);
    }
}

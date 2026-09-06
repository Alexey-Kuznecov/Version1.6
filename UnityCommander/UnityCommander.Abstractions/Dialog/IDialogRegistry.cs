

namespace UnityCommander.Abstractions.Dialog
{
    public interface IDialogRegistry : IOwnedRegistry
    {
        void Register(IDialogDefinition registration);

        bool Unregister(string id);

        bool TryGet(string id, out IDialogDefinition registration);

        bool TryGet<TDialog>(out IDialogDefinition registration);

        IReadOnlyCollection<IDialogDefinition> GetAll();
    }
}

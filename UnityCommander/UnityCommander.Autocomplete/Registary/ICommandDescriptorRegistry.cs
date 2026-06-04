using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Registary
{
    public interface ICommandDescriptorRegistry
    {
        void Register(ICommandDescriptor descriptor);

        IReadOnlyCollection<ICommandDescriptor> GetAll();

        ICommandDescriptor? Find(string name);
    }
}

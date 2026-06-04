using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Registary
{
    public sealed class CommandDescriptorRegistry
     : ICommandDescriptorRegistry
    {
        private readonly Dictionary<string, ICommandDescriptor> _descriptors =
            new(StringComparer.OrdinalIgnoreCase);

        public void Register(
            ICommandDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            _descriptors[descriptor.Name] = descriptor;
        }

        public ICommandDescriptor? Find(
            string name)
        {
            return _descriptors.GetValueOrDefault(name);
        }

        public IReadOnlyCollection<ICommandDescriptor> GetAll()
        {
            return _descriptors.Values;
        }
    }
}


namespace UnityCommander.Abstractions.Completion
{
    public interface ICommandCatalog
    {
        public IEnumerable<ICommandDescriptor> GetAll();
    }
}
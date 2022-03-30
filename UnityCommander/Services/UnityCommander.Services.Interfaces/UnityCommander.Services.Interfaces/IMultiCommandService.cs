
namespace UnityCommander.Services.Interfaces
{
    using Prism.Commands;

    /// <summary>
    /// The common status service.
    /// </summary>
    public interface IMultiCommandService
    {
        /// <summary>
        /// Gets the save command.
        /// </summary>
        CompositeCommand SaveCommand { get; }
    }
}


namespace UnityCommander.Services
{
    using Prism.Commands;

    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The common status service.
    /// </summary>
    public class GlobalCommandService : IGlobalCommandService
    {
        /// <summary>
        /// The save command.
        /// </summary>
        private static readonly CompositeCommand CommandRepository = new CompositeCommand();

        /// <summary>
        /// Gets the save command
        /// </summary>
        public CompositeCommand SaveCommand => CommandRepository;
    }
}

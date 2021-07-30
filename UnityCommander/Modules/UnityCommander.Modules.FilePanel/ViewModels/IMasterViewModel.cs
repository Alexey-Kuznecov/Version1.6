
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using UnityCommander.Core.Commands;

    /// <summary>
    /// The MasterViewModel interface.
    /// </summary>
    public interface IMasterViewModel
    {
        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        void SetCommandManager(CommandManager manager);
    }
}


namespace UnityCommander.Integration.Commands
{
    /// <summary>
    /// The CommandFactory interface.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// The command factory.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void CommandFactory(CommandBuilder command);
    }
}

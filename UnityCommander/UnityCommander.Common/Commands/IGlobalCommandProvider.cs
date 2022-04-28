
namespace UnityCommander.Common.Commands
{
    /// <summary>
    /// The GlobalCommandProvider interface.
    /// </summary>
    public interface IGlobalCommandProvider
    {
        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <returns>
        /// The <see cref="IGlobalCommandManager"/>.
        /// </returns>
        IGlobalCommandManager GetCommandManager();
    }
}

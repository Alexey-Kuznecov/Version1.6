
namespace UnityCommander.Core.IO
{
    /// <summary>
    /// The Commander interface.
    /// </summary>
    public interface ICommander
    {
        /// <summary>
        /// The get parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="ParametersBase"/>.
        /// </returns>
        ParametersBase GetParameters();

        /// <summary>
        /// The get manager.
        /// </summary>
        /// <returns>
        /// The <see cref="ManagerBase"/>.
        /// </returns>
        ManagerBase GetManager();
    } 
}

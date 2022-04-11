
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Input;

    /// <summary>
    /// The RibbonManager interface.
    /// </summary>
    public interface IRibbonManager
    {
        /// <summary>
        /// The manager.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void Collapse(ICommand command);

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="ribbon">
        /// The ribbon.
        /// </param>
        void Initial(Ribbon ribbon);
    }
}

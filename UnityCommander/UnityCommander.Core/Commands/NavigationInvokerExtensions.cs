
namespace UnityCommander.Core.Commands
{
    using UnityCommander.Core.Commands.Base;

    /// <summary>
    /// The navigation invoker extensions.
    /// </summary>
    public static class NavigationInvokerExtensions
    {
        /// <summary>
        /// The get path value.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPath(this Command command)
        {
            if (command is ConcreteCommand { Receiver: Navigator navigator })
            {
                return navigator.Path;
            }

            return null;
        }
    }
}

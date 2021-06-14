
namespace UnityCommander.Integration.Contracts
{
    using System;

    /// <summary>
    /// The data model of the host application command.
    /// </summary>
    public class HostAppCommand
    {
        /// <summary>
        /// Gets the target method name.
        /// </summary>
        public string Command { get; internal set; }

        /// <summary>
        /// Gets the delegate that is required to set the target command.
        /// </summary>
        public Type DelegateCommand { get; internal set; }

        /// <summary>
        /// Gets the type of object where the command is implemented.
        /// </summary>
        public Type SourceCommand { get; internal set; }
    }
}

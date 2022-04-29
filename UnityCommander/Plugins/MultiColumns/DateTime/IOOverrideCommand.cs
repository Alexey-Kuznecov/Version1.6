
namespace MultiColumns.DateTime
{
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The io override command.
    /// </summary>
    public class IoOverrideCommand : IOCommands
    {
        /// <summary>
        /// The file copy.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        [GlobalCommand("FileCopy2", CommandKeys.CtrlF)]
        public override void Move(string source, string destination)
        {
            base.Move(source, destination);
        }
    }
}

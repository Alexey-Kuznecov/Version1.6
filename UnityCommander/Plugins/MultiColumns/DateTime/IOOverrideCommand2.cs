
namespace MultiColumns.DateTime
{
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The io override command.
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public class IOOverrideCommand2 : IOCommands
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
        [GlobalCommand("MultiColumns File Moving", CommandKeys.CtrlB)]
        public override void Move(string source, string destination)
        {
            base.Move(source, destination);
        }
    }
}

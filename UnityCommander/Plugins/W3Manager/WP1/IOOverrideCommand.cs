
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The io override command.
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public class IOOverrideCommand : IOCommands
    {
        /// <summary>
        /// The move.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        [GlobalCommand("W3Manager File Moving", CommandKeys.CtrlD)]
        public override void Move(string source, string destination)
        {
            base.Move(source, destination);
        }

        /// <summary>
        /// The file copy.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        [GlobalCommand("W3Manager File Moving", CommandKeys.CtrlB)]
        public override void Delete(string source)
        {
            base.Delete(source);
        }
    }
}

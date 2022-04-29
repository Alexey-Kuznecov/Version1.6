
namespace W3Manager.WP1
{
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The io override command.
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public class IOOverrideCommand : IOCommands
    {
        /// <summary>
        /// The test.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="isShadowCopy">
        /// The is shadow copy.
        /// </param>
        /// <param name="baseCommand">
        /// The base command.
        /// </param>
        [GlobalCommand("W3Manager Test", CommandKeys.CtrlB)]
        public override void Test(string source, string destination, bool isShadowCopy, BaseCommand baseCommand)
        {
            base.Test(source, destination, isShadowCopy, baseCommand);
        }

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
    }
}

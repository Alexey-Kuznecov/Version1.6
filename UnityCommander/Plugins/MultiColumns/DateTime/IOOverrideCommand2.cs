
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
        [GlobalCommand("MultiColumns File Delete", CommandKeys.CtrlB)]
        public override void Delete(string source)
        {
            base.Delete(source);
        }
    }
}

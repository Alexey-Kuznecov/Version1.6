
using UnityCommander.Common;

namespace UnityCommander.Integration.Commands
{
    /// <summary>
    /// The io commands.
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public class IOCommands : BaseCommand, IFileOperations
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
        [GlobalCommand(CommandNames.FileMove, CommandKeys.CtrlG)]
        public virtual void Move(string source, string destination)
        {
            //Core.IO.Operations.FileManager.Move(source, destination);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        [GlobalCommand(CommandNames.FileDelete, CommandKeys.CtrlH)]
        public virtual void Delete(string source)
        {
            //Core.IO.Operations.FileManager.Delete(source);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dirName">
        /// The dir name.
        /// </param>
        [GlobalCommand(CommandNames.FileCreate, CommandKeys.CtrlI)]
        public virtual void Create(string dirName)
        {
            //Core.IO.Operations.FileManager.Create(dirName);
        }
    }
}

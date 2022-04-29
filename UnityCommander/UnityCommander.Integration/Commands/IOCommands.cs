
namespace UnityCommander.Integration.Commands
{
    using UnityCommander.Common.Commands;
    using UnityCommander.Core.IO.Operations;
    using UnityCommander.Integration.Enums;

    // ReSharper disable once InconsistentNaming
    public abstract class IOCommands : FileManager
    {
        public virtual void Move(string source, string destination)
        {
            base.Move(source, destination);
        }

        public virtual void Delete(string source)
        {
            base.Delete(source);
        }
    }
}

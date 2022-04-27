
namespace UnityCommander.Integration.Commands
{
    // ReSharper disable once InconsistentNaming
    public abstract class IOCommands : BaseCommand
    {
        public virtual void Test(string source, string destination, bool IsShadowCopy, BaseCommand baseCommand)
        {
        }

        public virtual void Move(string source, string destination)
        {
        }

        public virtual void FileCopy(string source, string destination)
        {
        }

        public virtual void Delete(string source)
        {
        }
    }
}

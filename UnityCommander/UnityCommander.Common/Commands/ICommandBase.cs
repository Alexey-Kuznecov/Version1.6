
namespace UnityCommander.Common.Commands
{
    using System.Windows.Input;

    public interface ICommandBase
    {
        public string Name { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
    }
}

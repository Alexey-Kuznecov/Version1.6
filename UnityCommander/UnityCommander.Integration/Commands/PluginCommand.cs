
namespace UnityCommander.Integration.Commands
{
    using System.Windows.Input;

    using UnityCommander.Common.Commands;

    public class PluginCommand : ICommandBase
    {
        public string Name { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }
    }
}

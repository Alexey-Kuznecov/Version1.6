using System;
using System.Windows.Input;

namespace UnityCommander.Integration.Commands
{
    public class GlobalCommand
    {
        public string CommandName { get; set; }

        public string DisplayName { get; set; }
        
        public string Description { get; set; }

        public CommandSource CommandSource { get; set; }

        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }

        public Type Source { get; set; }

        public Delegate Delegate { get; set; }

        public GlobalCommand(string commandName)
        {
            this.CommandName = commandName;
        }

        public GlobalCommand()
        {
        }
    }
}

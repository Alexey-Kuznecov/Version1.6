using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnityCommander.Common.Models
{
    public class UCCommand
    {
        public string Name { get; set; }

        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }

        public UCCommand(string name, ICommand command, InputGesture gesture)
        {
            this.Command = command;
            this.ShortcutKey = gesture;
            this.Name = name;
        }
    }
}

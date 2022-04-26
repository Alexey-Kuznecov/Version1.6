using System;
using System.Windows.Input;

namespace UnityCommander.Integration.Commands
{
    public class GlobalCommand
    {
        public string Name { get; set; }

        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }

        public Type Source { get; set; }

        public Delegate Delegate { get; set; }

        public BaseCommand BaseCommand { get; set; }

        public GlobalCommand(string name, ICommand command, InputGesture gesture, Delegate @delegate, Type source)
        {
            this.Command = command;
            this.ShortcutKey = gesture;
            this.Name = name;
            this.Source = source;
            this.Delegate = @delegate;
        }
    }
}

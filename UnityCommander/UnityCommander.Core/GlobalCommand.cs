
namespace UnityCommander.Core
{
    using System;
    using System.Windows.Input;

    using UnityCommander.Common.Commands;

    /// <summary>
    /// The global command.
    /// </summary>
    public class GlobalCommand : IGlobalCommand
    {
        public string DisplayName { get; set; }
        
        public string Description { get; set; }

        public string Name { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public InputGesture ShortcutKey { get; set; }

        public Type Source { get; set; }

        public Delegate Delegate { get; set; }
    }
}

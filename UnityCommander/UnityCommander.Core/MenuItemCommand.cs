using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Common.Commands;

namespace UnityCommander.Core
{
    public class MenuItemCommand : IGlobalCommand
    {
        public string DisplayName { get; set; }
        public InputGesture ShortcutKey { get; set; }
        public string Name { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
    }
}

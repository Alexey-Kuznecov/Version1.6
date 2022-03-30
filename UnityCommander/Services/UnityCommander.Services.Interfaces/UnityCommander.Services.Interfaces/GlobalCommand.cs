using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnityCommander.Services
{
    public class GlobalCommand
    {
        public ICommand Command { get; set; }

        public InputGesture ShortcutKey { get; set; }
    }
}

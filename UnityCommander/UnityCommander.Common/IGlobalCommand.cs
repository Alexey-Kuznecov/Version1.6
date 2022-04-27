using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnityCommander.Common
{
    public interface IGlobalCommand
    {
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
    }
}

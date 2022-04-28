using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Shapes;
using UnityCommander.Common;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Controls.Ribbon
{
    using UnityCommander.Common.Commands;

    public class IRibbonCommand : IGlobalCommand
    {
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
    }
}

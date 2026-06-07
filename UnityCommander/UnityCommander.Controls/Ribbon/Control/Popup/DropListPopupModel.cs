using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Shapes;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class DropListPopupModel
    {
        public ICommand Command { get; set; }
        public Path Icon { get; set; }
        public object Content { get; set; }
    }
}

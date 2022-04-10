using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Shapes;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class RibbonControlModel
    {
        public object Content { get;set; }

        /// <summary>
        /// Gets or sets the buttonIcon.
        /// </summary>
        public Path Icon { get; set; }

        /// <summary>
        /// Gets or sets the buttonCommand.
        /// </summary>
        public ICommand Command { get; set; }
    }
}

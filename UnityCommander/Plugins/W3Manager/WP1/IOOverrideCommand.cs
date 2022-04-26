using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Commands;

namespace W3Manager.WP1
{
    // ReSharper disable once InconsistentNaming
    public class IoOverrideCommand : IOCommands
    {
        [GlobalCommand("FileCopy")]
        public override void FileCopy(string source, string destination)
        {
            base.FileCopy(source, destination);
        }
    }
}

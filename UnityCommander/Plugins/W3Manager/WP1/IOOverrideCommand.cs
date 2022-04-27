using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;

namespace W3Manager.WP1
{
    // ReSharper disable once InconsistentNaming
    public class IoOverrideCommand : IOCommands
    {
        [GlobalCommand("FileCopy2", CommandKeys.CtrlB)]
        public override void FileCopy(string source, string destination)
        {
            base.FileCopy(source, destination);
        }

        [GlobalCommand("FileMove", CommandKeys.CtrlC)]
        public override void Move(string source, string destination)
        {
            base.Move(source, destination);
        }
    }
}

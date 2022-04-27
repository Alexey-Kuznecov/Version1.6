using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;

namespace MultiColumns.DateTime
{
    // ReSharper disable once InconsistentNaming
    public class IoOverrideCommand : IOCommands
    {
        [GlobalCommand("FileCopy", CommandKeys.CtrlG)]
        public override void FileCopy(string source, string destination)
        {
            base.FileCopy(source, destination);
        }
    }
}

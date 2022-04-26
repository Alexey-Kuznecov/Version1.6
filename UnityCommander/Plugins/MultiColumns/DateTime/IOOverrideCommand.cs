using UnityCommander.Integration.Commands;

namespace MultiColumns.DateTime
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

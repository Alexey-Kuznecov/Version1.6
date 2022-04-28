
namespace W3Manager.WP1
{
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Enums;

    // ReSharper disable once InconsistentNaming
    public class IoOverrideCommand : IOCommands
    {
        [GlobalCommand("Test1", CommandKeys.CtrlZ)]
        public override void Test(string source, string destination, bool IsShadowCopy, BaseCommand baseCommand)
        {
            base.Test(source, destination, IsShadowCopy, baseCommand);
        }

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

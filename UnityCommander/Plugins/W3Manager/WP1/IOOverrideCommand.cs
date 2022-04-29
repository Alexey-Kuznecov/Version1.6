
namespace W3Manager.WP1
{
    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;

    // ReSharper disable once InconsistentNaming
    public class IoOverrideCommand : IOCommands
    {
        [GlobalCommand("Test1", CommandKeys.CtrlB)]
        public override void Test(string source, string destination, bool IsShadowCopy, BaseCommand baseCommand)
        {
            base.Test(source, destination, IsShadowCopy, baseCommand);
        }

        [GlobalCommand("FileCopy2", CommandKeys.CtrlD)]
        public override void Move(string source, string destination)
        {
            base.Move(source, destination);
        }
    }
}

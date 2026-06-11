
namespace MultiColumns.DateTime
{
    using UnityCommander.Integration.Commands;

    public class IOOverrideCommand2 : IOCommands
    {
        [GlobalCommand("MultiColumns File Delete", CommandKeys.CtrlB)]
        public override void Delete(string source)
        {
            base.Delete(source);
        }
    }
}


namespace UnityCommander.Common.Commands
{
    using System;

    public class BaseCommand
    {
        internal Type Source { get; set; }
        
        internal object Priority { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Integration.Commands
{
    public class BaseCommand
    {
        internal Type Source { get; set; }
        
        internal object Priority { get; set; }
    }
}

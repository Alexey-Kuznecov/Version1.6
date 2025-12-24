using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.CLI.Input
{
    public enum TokenKind
    {
        Command,
        Argument,
        Path,
        Unknown
    }
}

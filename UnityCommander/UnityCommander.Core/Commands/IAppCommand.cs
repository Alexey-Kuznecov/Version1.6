using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.Commands
{
    public interface IAppCommand
    {
        void Execute();
        void Undo();
    }
}

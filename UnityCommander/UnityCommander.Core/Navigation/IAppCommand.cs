
using System;

namespace UnityCommander.Core.Navigation
{
    [Obsolete]
    public interface IAppCommand
    {
        void Execute();
        void Undo();
    }
}

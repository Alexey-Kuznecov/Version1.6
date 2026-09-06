
using System.Collections.Generic;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public interface IConsoleProfileStore 
    { 
        IReadOnlyList<ConsoleProfile> Load(); 
        void Save(ConsoleProfile profile); 
    }
}

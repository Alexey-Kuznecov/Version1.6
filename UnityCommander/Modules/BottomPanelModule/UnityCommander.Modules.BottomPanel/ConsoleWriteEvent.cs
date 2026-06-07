
using Prism.Events;

namespace UnityCommander.Modules.BottomPanel
{
    public class ConsoleWriteEvent : PubSubEvent<string> { }
    
    public class ConsoleClearEvent : PubSubEvent { }       
}

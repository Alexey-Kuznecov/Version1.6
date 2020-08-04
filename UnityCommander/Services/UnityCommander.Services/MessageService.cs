using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}

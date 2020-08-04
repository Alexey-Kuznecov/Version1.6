using UnityCommander.Services.DataProvider.Interfaces;

namespace UnityCommander.Services.DataProvider
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}


namespace UnityCommander.Services
{
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The message service.
    /// </summary>
    public class MessageService : IMessageService
    {
        /// <summary>
        /// The get message.
        /// </summary>
        /// <returns> The <see cref="string"/>. </returns>
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}

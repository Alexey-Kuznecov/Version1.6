
namespace UnityCommander.Common.Commands
{
    /// <summary>
    /// Идея этого интерфейса в том, чтобы обеспечить возможность программе,
    /// при определенных условиях запрашивать у плагина проверку или обновления
    /// данных, которые он предоставил. Например, в программе были изменены настройки плагина.
    /// </summary>
    public interface IRepeatQuery
    {
        /// <summary>
        /// The update.
        /// </summary>
        public void Update();
        
        /// <summary>
        /// The validate.
        /// </summary>
        public void Validate();
    }
}

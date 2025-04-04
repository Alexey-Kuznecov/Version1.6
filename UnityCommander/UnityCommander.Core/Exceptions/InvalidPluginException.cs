using System;

namespace UnityCommander.Core.Exceptions
{
    /// <summary>
    /// Исключение, возникающее при загрузке плагина, если файл отсутствует, повреждён
    /// или не соответствует ожидаемому формату.
    /// </summary>
    public class InvalidPluginException : Exception
    {
        /// <summary>
        /// Создаёт исключение <see cref="InvalidPluginException"/> без сообщения.
        /// </summary>
        public InvalidPluginException() { }

        /// <summary>
        /// Создаёт исключение <see cref="InvalidPluginException"/> с указанным сообщением.
        /// </summary>
        /// <param name="message">Описание ошибки.</param>
        public InvalidPluginException(string message)
            : base(message) { }

        /// <summary>
        /// Создаёт исключение <see cref="InvalidPluginException"/> с указанным сообщением и внутренним исключением.
        /// </summary>
        /// <param name="message">Описание ошибки.</param>
        /// <param name="innerException">Внутреннее исключение, вызвавшее данную ошибку.</param>
        public InvalidPluginException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
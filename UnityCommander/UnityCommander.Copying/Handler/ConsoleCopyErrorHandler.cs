
using UnityCommander.CLI.Core;

namespace UnityCommander.Copying.Handler
{
    public class ConsoleCopyErrorHandler : ICopyErrorHandler
    {
        private readonly IConsoleOutput _output;

        public ConsoleCopyErrorHandler(IConsoleOutput output)
        {
            _output = output;
        }

        public bool HandleError(FileCopyErrorContext context)
        {
            _output.WriteWarning($"Пропущен файл: {context.SourcePath}\nПричина: {context.Exception.Message}");
            return true; // продолжать
        }
    }
}

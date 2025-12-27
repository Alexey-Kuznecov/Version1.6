using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Logging.Abstractions.Console
{
    public static class ConsoleExtensions
    {
        public static void WriteLine(this IConsole console, string text) =>
            console.Write(new ConsoleMessage(text));

        public static void Error(this IConsole console, string text) =>
            console.Write(new ConsoleMessage(text, ConsoleMessageKind.Error));

        public static void Warning(this IConsole console, string text) =>
            console.Write(new ConsoleMessage(text, ConsoleMessageKind.Warning));

        public static void Success(this IConsole console, string text) =>
            console.Write(new ConsoleMessage(text, ConsoleMessageKind.Success));
    }
}

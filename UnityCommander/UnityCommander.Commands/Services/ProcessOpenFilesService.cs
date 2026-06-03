
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Parsing;
using UnityCommander.Native;

namespace UnityCommander.Commands.Services
{
    public sealed class ProcessOpenFilesService
       : IProcessOpenFilesService
    {
        public async Task RunAsync(
            IConsoleOutput output,
            IArgumentCollection arguments,
            CancellationToken token)
        {
            string processName =
                arguments.GetString("name");

            try
            {
                output.Clear();

                var process = Process.GetProcessesByName(processName).FirstOrDefault();

                if (process == null)
                {
                    output.WriteLine($"Процесс с именем '{processName}' не найден.");
                    return;
                }

                output.WriteLine($"Открытые файлы процесса: {processName} (PID: {process.Id})");

                List<FileSystemInfo> infos = new();

                try
                {
                    using var enumerator = ProcessUtility.GetOpenFilesEnumerator(process.Id);
                    while (enumerator.MoveNext())
                    {
                        if (token.IsCancellationRequested)
                            break;

                        infos.Add(enumerator.Current);
                    }
                }
                catch (Exception ex)
                {
                    output.WriteLine($"Ошибка при получении открытых файлов: {ex.Message}");
                    return;
                }

                var sorted = infos.OrderBy(f => f.FullName, StringComparer.OrdinalIgnoreCase);
                foreach (var file in sorted)
                {
                    output.WriteLine(file.FullName);
                }
            }
            catch (TaskCanceledException)
            {
                // Задача была отменена, корректно завершаем выполнение
                output.Clear();
                output.WriteLine("Операция завешена.");
            }
        }
    }
}

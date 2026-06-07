
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Performance;
using UnityCommander.Commands.Rendering;

namespace UnityCommander.Commands.Services
{
    public sealed class SysStatService
      : ISysStatService
    {
        private readonly IConsoleRenderer<SystemStats>
            _renderer;

        public SysStatService(
            IConsoleRenderer<SystemStats> renderer)
        {
            PhysicalDiskResolver.Initialize();
            _renderer = renderer;
        }

        public async Task RunAsync(
            IConsoleOutput output,
            IArgumentCollection arguments,
            CancellationToken token)
        {
            int interval =
                arguments.GetInt("interval", 1);

            string disk =
                arguments.GetString("disk")
                ?? "_Total";

            using var monitor =
                new SystemPerformanceMonitor(disk);

            while (!token.IsCancellationRequested)
            {
                var stats =
                    monitor.GetStats();

                _renderer.Render(
                    output,
                    stats);

                await Task.Delay(
                    TimeSpan.FromSeconds(interval),
                    token);
            }

            output.Clear();

            output.WriteLine(
                "Операция была отменена.");
        }
    }
}

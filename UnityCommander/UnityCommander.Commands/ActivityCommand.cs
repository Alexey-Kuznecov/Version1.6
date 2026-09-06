
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Background;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;

namespace UnityCommander.Commands
{
    [ConsoleCommand(
      "activity",
      "Отслеживает состояние активности пользователя.")]
    public sealed class ActivityCommand : IConsoleCommand
    {
        private readonly IUserActivityService _activity;

        private readonly IBackgroundResourcePolicy _policy;

        public IConsoleOutput _output;

        public string Name => "activity";

        public string Description =>
            "Отслеживает состояние активности пользователя.";

        private bool _monitoring;

        public ActivityCommand(
            IUserActivityService activity,
            IBackgroundResourcePolicy policy)
        {
            _activity = activity;
            _policy = policy;
        }

        public Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            _output = context.Output;

            if (_monitoring)
            {
                context.Output.WriteLine(
                    "User activity monitoring is already enabled.");

                return Task.CompletedTask;
            }

            _monitoring = true;

            _activity.StateChanged += OnStateChanged;

            context.Output.WriteLine(
                $"Activity monitoring enabled. Current state: {_activity.State}");

            return Task.CompletedTask;
        }

        private void OnStateChanged(
            object? sender,
            UserActivityState state)
        {
            _output.WriteLine(
                $"User Activity : {_activity.State}");

            _output.WriteLine(
                $"Background    : {_policy.Priority}");
        }

        public Task FinalizeAsync()
        {
            _activity.StateChanged -= OnStateChanged;
            _monitoring = false;

            return Task.CompletedTask;
        }
    }
}


using Prism.Mvvm;
using System;
using UnityCommander.CLI.Infrastructure;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleActivityState : BindableBase, IConsoleActivityState
    {
        public string Title { get; set; } = "";

        private string _status = "";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private long _found = 0;
        public long Found
        {
            get => _found;
            set => SetProperty(ref _found, value);
        }

        private long _processed = 0;
        public long Processed
        {
            get => _processed;
            set
            {
                if (SetProperty(ref _processed, value))
                {
                    RaisePropertyChanged(nameof(Progress));
                }
            }
        }

        private long _skipped = 0;
        public long Skipped
        {
            get => _skipped;
            set => SetProperty(ref _skipped, value);
        }

        private long? _total = 0;
        public long? Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        private TimeSpan _elapsed = TimeSpan.Zero;
        public TimeSpan Elapsed
        {
            get => _elapsed;
            set => SetProperty(ref _elapsed, value);
        }

        public double? Progress =>
            Total is > 0
                ? (double)Processed / Total.Value
                : null;
    }
}

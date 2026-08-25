
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityCommander.Abstractions.Diagnostic;
using UnityCommander.Logging;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Core.Navigation
{
    public class NavigationManager
    {
        private readonly Stack<string> _back = new();
        private readonly Stack<string> _forward = new();

        public string? Current { get; private set; }
        
        private readonly Func<string?, bool> _pathValidator;
        
        private readonly LoggerCreator _loggerCreator;

        private readonly ILogger _logger;
       
        public event Action<string?>? CurrentChanged;

        public NavigationManager(Func<string?, bool>? pathValidator = null)
        {
            _loggerCreator = Log.GetLoggerCreator();

            this._logger = Log.Create(
                "Performance", 
                LogScope.Runtime);

            if (pathValidator != null)
                _pathValidator = pathValidator;
            else
                _pathValidator = path => Directory.Exists(path) || VirtualPaths.MyComputer == path;

            Current = null;
        }

        public bool IsValidPath(string? path) => _pathValidator(path);

        public bool TryNavigateTo(string? path, bool forceRecord = false)
        {
            using (_loggerCreator.ProfileScope(LogScope.UI, "Navigation"))
            {
                if (!IsValidPath(path)) return false;
                NavigateTo(path, forceRecord);
            }

            return true;
        }

        public void NavigateTo(
            string? path,
            bool recordOnSame = false)
        {
            var sw = Stopwatch.StartNew();

            var valid = IsValidPath(path);

            var validationMs = sw.Elapsed.TotalMilliseconds;

            if (!valid)
                throw new InvalidOperationException(
                    $"Path is invalid: {path ?? "<root>"}");

            if (!recordOnSame && Equals(Current, path))
                return;

            var old = Current;

            if (Current != null)
                _back.Push(Current);

            Current = path;

            var stateMs = sw.Elapsed.TotalMilliseconds;

            _forward.Clear();

            var historyMs = sw.Elapsed.TotalMilliseconds;

            CurrentChanged?.Invoke(Current);

            var totalMs = sw.Elapsed.TotalMilliseconds;

            //_logger.Info(
            //     $"\n[Navigation] " +
            //     $"\nFrom='{old ?? "<null>"}' " +
            //     $"\nTo='{path ?? "<null>"}' " +
            //     $"\nValidation={validationMs:F2}ms " +
            //     $"\nState={stateMs - validationMs:F2}ms " +
            //     $"\nHistory={historyMs - stateMs:F2}ms " +
            //     $"\nCurrentChanged={totalMs - historyMs:F2}ms " +
            //     $"\nTotal={totalMs:F2}ms " +
            //     $"\nBack={_back.Count} " +
            //     $"\nForward={_forward.Count}");
        }

        public bool CanGoBack => _back.Count > 0;
        public bool CanGoForward => _forward.Count > 0;

        public void GoBack()
        {
            if (!CanGoBack) return;
            _forward.Push(Current);
            Current = _back.Pop();
            CurrentChanged?.Invoke(Current);
        }

        public void GoForward()
        {
            if (!CanGoForward) return;
            _back.Push(Current);
            Current = _forward.Pop();
            CurrentChanged?.Invoke(Current);
        }

        public void ClearHistory()
        {
            _back.Clear();
            _forward.Clear();
        }
    }
}

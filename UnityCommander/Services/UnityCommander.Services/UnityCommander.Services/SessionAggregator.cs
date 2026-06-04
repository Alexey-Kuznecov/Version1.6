
using System;
using System.Collections.Generic;
using UnityCommander.Common.State;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class SessionAggregator : ISessionAggregator
    {
        private readonly List<Action<AppSessionState>> _captures = new();

        private readonly List<Action<AppSessionState>> _restores = new();

        public void RegisterCapture(
            Action<AppSessionState> capture)
        {
            _captures.Add(capture);
        }

        public void RegisterRestore(
            Action<AppSessionState> restore)
        {
            _restores.Add(restore);
        }

        public void Capture(AppSessionState state)
        {
            foreach (var capture in _captures)
            {
                capture(state);
            }
        }

        public void Restore(AppSessionState state)
        {
            foreach (var restore in _restores)
            {
                restore(state);
            }
        }
    }
}

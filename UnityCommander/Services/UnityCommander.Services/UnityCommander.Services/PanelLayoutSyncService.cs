
using System;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PanelLayoutSyncService : IPanelLayoutSyncService
    {
        public event Action<double> SplitterChanged;

        private double _splitterPosition;

        public double SplitterPosition
        {
            get => _splitterPosition;
            set
            {
                if (_splitterPosition == value)
                    return;

                _splitterPosition = value;
                SplitterChanged?.Invoke(value);
            }
        }

        public void Update(double value)
        {
            SplitterPosition = value;
        }
    }
}

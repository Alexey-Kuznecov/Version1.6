using System;
using System.ComponentModel;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnModel : INotifyPropertyChanged
    {
        public string Id { get; init; }
        public string Header { get; init; }
        public string CellTemplateResourceKey { get; init; }
        public string DisplayMemberPath { get; init; }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (Math.Abs(_width - value) > 0.0001)
                {
                    _width = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
                }
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        public int Order { get; init; }
        public string SyncGroup { get; init; }
        public bool ForFiles { get; init; } = true;
        public bool ForFolders { get; init; } = true;
        public bool ForDrives { get; init; } = true;

        // Optional: small value-provider when you don't want DataTemplate
        public Func<object, object> ColumnValueHandler { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}


using System.Windows.Input;
using UnityCommander.Modules.StatusBar.Services;

namespace UnityCommander.Common.StatusBar
{
    public class CopyProgressItem : IStatusBarItem
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }

        public double Progress { get; set; }

        public long Speed { get; set; }

        public string Title { get; set; }

        public object Icon { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public ICommand ClickCommand { get; set; }

        public object Details { get; set; }
    }
}

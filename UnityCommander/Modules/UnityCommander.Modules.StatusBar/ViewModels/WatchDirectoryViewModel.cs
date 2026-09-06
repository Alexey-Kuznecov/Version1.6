
namespace UnityCommander.Modules.StatusBar.ViewModels
{
    internal class WatchDirectoryViewModel : BindableBase
    {
        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                if (SetProperty(ref _message, value))
                {
                }
            }
        }

        public WatchDirectoryViewModel()
        {
            Message = "Watch files...";
        }
    }
}

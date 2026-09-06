
using Prism.Mvvm;

namespace SidebarExtensions
{
    public class SidebarGitViewModel : BindableBase
    {
        private string _message;

        public SidebarGitViewModel()
        {
            Message = "Hello from SidebarGitViewModel!";
        }

        public string Message
        {
            get => _message;
            set
            {
                SetProperty(ref this._message, value);
            }
        }
    }
}

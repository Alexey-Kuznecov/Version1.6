
using Prism.Mvvm;

namespace UnityCommander.Modules.WebBrowser.ViewModels
{
    public class WebBrowserViewModel : BindableBase
    {
        private string message;
        
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public WebBrowserViewModel()
        {
            Message = "Здесь будет веб-браузер";
        }
    }
}

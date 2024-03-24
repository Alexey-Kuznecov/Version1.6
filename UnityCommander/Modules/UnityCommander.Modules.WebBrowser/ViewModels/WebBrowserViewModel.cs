using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

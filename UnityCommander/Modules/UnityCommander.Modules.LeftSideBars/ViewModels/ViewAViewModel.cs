using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using UnityCommander.Business;
using UnityCommander.Core;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        IEventAggregator _ea;
        public DelegateCommand SendCopyInfoCommand { get; private set; }

        public ViewAViewModel(IEventAggregator ea)
        {
            this._ea = ea;
            this.SendCopyInfoCommand = new DelegateCommand(SendCopyInfo);
        }

        private void SendCopyInfo()
        {
            _ea.GetEvent<MessageSendEvent>().Publish("It is works!");
        }
    }
}

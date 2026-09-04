using IconMaker.Core.Mvvm.Base;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace IconBrowser.Components.InputBox
{
    public class InputBoxViewModel : PropertiesChanged
    {
        internal static List<string> ForbidWord = new List<string>(new[] { "dick", "fool", "Bitch", string.Empty });

        private string text;

        private Actions userAction;

        public InputBoxViewModel(ICommand action, Actions actionType, string text)
        {
            ForbidWord.Add(text);
            Text = text;
            UserAction = actionType;
            Action = action;
        }

        public Actions UserAction
        {
            get => this.userAction;
            set
            {
                this.userAction = value;
                SetProperty(ref this.userAction, value, () => UserAction);
            }
        }

        public bool IsEnableAction { get; set; } = true;

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                //IsEnableAction = !ForbidWord.Contains(this.text);

                OnPropertyChanged("Text");
                //OnPropertyChanged("IsEnableAction");
            }
        }

        public ICommand Action { get; set; }

        public ICommand Cancel => new RelayCommand(obj =>
        {
            Window win = (Window) obj;
            win.Visibility = Visibility.Hidden;
        });
    }
}

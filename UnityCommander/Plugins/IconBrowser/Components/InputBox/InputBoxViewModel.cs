using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AIconBrowser.Mvvm.Base;

namespace AIconBrowser.Components.InputBox
{
    /// <summary>
    /// View model for the input window.
    /// </summary>
    public class InputBoxViewModel : PropertiesChanged
    {
        /// <summary>
        /// The forbid word.
        /// </summary>
        internal static List<string> ForbidWord = new List<string>(new[] { "dick", "fool", "Bitch", string.Empty });

        /// <summary>
        /// The text.
        /// </summary>
        private string text;

        /// <summary>
        /// The user action.
        /// </summary>
        private Actions userAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBoxViewModel"/> class.
        /// </summary>
        /// <param name="text"> Text to be transferred to the window text box. </param>
        /// <param name="action"> Action that to be performed. </param>
        /// <param name="actionType"> Action type that be interpreted how button press. </param>
        public InputBoxViewModel(ICommand action, Actions actionType, string text)
        {
            ForbidWord.Add(text);
            Text = text;
            UserAction = actionType;
            Action = action;
        }

        /// <summary>
        /// Gets or sets the name of the action
        /// that the user must perform when the button is clicked.
        /// </summary>
        public Actions UserAction
        {
            get => this.userAction;
            set
            {
                this.userAction = value;
                SetProperty(ref this.userAction, value, () => UserAction);
            }
        }

        /// <summary>
        /// Locks the button if the ListBox contains an exception name, and unlocks if it does not.
        /// </summary>
        public bool IsEnableAction { get; set; } = true;

        /// <summary>
        /// Gets or sets the name that the user enters the window field
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                IsEnableAction = !ForbidWord.Contains(this.text);

                OnPropertyChanged("Text");
                OnPropertyChanged("IsEnableAction");
            }
        }

        /// <summary>
        /// Gets or set the command is responsible for the action to be performed.
        /// </summary>
        public ICommand Action { get; set; }

        /// <summary>
        /// Gets or set the command is responsible for safely closing the name entry window.
        /// </summary>
        public ICommand Cancel => new RelayCommand(obj =>
        {
            Window win = (Window) obj;
            win.Visibility = Visibility.Hidden;
        });
    }
}

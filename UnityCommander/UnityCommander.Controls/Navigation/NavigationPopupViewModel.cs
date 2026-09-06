
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace UnityCommander.Controls.Navigation
{
    internal class NavigationPopupViewModel : BindableBase
    {
        private bool _popButtonIsEnabled;

        private bool _popupIsOpen;

        private readonly ICommand _navigateCommand;

        public NavigationPopupViewModel(
            string currentPath,
            ICommand navigateCommand)
        {
            _navigateCommand = navigateCommand;
            
            PopupIsOpen = true;

            DirectoryList = new ObservableCollection<NavigationPopupItem>();

            DirectoryInfo dir = new DirectoryInfo(currentPath);

            foreach (var item in dir.GetDirectories())
            {
                if ((item.Attributes & FileAttributes.Hidden) == 0)
                    DirectoryList.Add(new NavigationPopupItem()
                    {
                        SelectedPath = item.FullName
                    });
            }
        }

        public ObservableCollection<NavigationPopupItem> DirectoryList { get; }

        public bool PopupIsOpen
        {
            get => _popupIsOpen;
            set
            {
                SetProperty(ref _popupIsOpen, value);
                PopButtonIsEnabled = !value;
            }
        }

        public NavigationPopupItem? SelectedItem
        {
            set
            {
                if (value != null)
                    _navigateCommand.Execute(value.SelectedPath);
            }
        }

        public bool PopButtonIsEnabled
        {
            get => _popButtonIsEnabled;
            set => this.SetProperty(ref _popButtonIsEnabled, value);
        }
    }
}

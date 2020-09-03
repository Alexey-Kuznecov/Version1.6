using System;
using System.IO;
using Prism.Mvvm;
using System.Security.AccessControl;
using System.Collections.ObjectModel;
using UnityCommander.Core.IO;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class NTFSSecurityViewModel : BindableBase
    {
        private ObservableCollection<string> _account;

        public NTFSSecurityViewModel(string path)
        {
            FileInfo info = new FileInfo("");
            NTFSSecurity security = new NTFSSecurity(path);
            var ntfsAccounts = NTFSSecurity.GetNTAccounts(path);

            foreach (var account in ntfsAccounts)
            {
                NTAccount.Add(account.IdentityReference.Value.Split('\\',1).ToString());
            }
        }

        public ObservableCollection<string> NTAccount 
        { 
            get => this._account;
            set => SetProperty(ref this._account, value);
        }
    }
}

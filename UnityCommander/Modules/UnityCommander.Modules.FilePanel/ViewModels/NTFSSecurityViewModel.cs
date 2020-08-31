using System;
using System.IO;
using Prism.Mvvm;
using System.Security.AccessControl;
using System.Collections.ObjectModel;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class NTFSSecurityViewModel : BindableBase
    {
        private ObservableCollection<string> _account;

        public NTFSSecurityViewModel()
        {
            FileInfo info = new FileInfo("");
            FileSecurity accessControl = info.GetAccessControl(AccessControlSections.All);
            var s = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            //accessControl.RemoveAccessRule(
            //    new FileSystemAccessRule(
            //        Environment.UserName.ToString(),
            //        FileSystemRights.FullControl,
            //        AccessControlType.Deny));
        }

        public ObservableCollection<string> Account 
        { 
            get => this._account;
            set => SetProperty(ref this._account, value);
         }
    }
}

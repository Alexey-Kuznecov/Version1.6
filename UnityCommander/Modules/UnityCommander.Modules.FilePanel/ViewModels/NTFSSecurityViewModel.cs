
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System.Collections.ObjectModel;
    using System.IO;

    using Prism.Mvvm;
    using UnityCommander.Core.IO;

    /// <summary>
    /// The ntfs security view model.
    /// </summary>
    public class NTFSSecurityViewModel : BindableBase
    {
        /// <summary>
        /// The account.
        /// </summary>
        private ObservableCollection<string> account;

        /// <summary>
        /// Initializes a new instance of the <see cref="NTFSSecurityViewModel"/> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public NTFSSecurityViewModel(string path)
        {
            FileInfo info = new FileInfo("");
            NTFSSecurity security = new NTFSSecurity(path);
            var ntfsAccounts = NTFSSecurity.GetNTAccounts(path);

            foreach (var account in ntfsAccounts)
            {
                this.NTAccount.Add(account.IdentityReference.Value.Split('\\', 1).ToString());
            }
        }

        /// <summary>
        /// Gets or sets the nt account.
        /// </summary>
        public ObservableCollection<string> NTAccount 
        { 
            get => this.account;
            set => this.SetProperty(ref this.account, value);
        }
    }
}

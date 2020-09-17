
namespace UnityCommander.Core.IO
{
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.Security.Principal;

    /// <summary>
    /// The ntfs account model.
    /// </summary>
    public class NTFSAccountModel
    {
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        public IdentityReference Owner { get; set; }

        /// <summary>
        /// Gets or sets the accounts.
        /// </summary>
        public List<FileSystemAccessRule> Accounts { get; set; }
    }
}
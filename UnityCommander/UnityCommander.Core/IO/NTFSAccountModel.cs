using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UnityCommander.Core.IO
{
    public class NTFSAccountModel
    {
        public IdentityReference Owner { get; set; }
        public List<FileSystemAccessRule> Accounts { get; set; }
    }
}
using System;
using System.IO;
using System.Security.AccessControl;

namespace UnityCommander.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo info = new FileInfo("h:\\Works\\UnitTests\\Source\\Films\\Child");
            FileSecurity accessControl = info.GetAccessControl(AccessControlSections.All);
            var s = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
        }
    }
}

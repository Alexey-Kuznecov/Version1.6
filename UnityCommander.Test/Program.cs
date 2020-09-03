using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.AccessControl;

namespace UnityCommander.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "h:\\Works\\UnitTests\\Source\\Films\\Child\\img0.dv";
            NTFSSecurity security = new NTFSSecurity(path);
            string ntfsAccountName = "Alexey"; // "Alexey", "Администраторы";
            string argumnet = string.Concat<string>(args);
 
            if (argumnet.Contains("-TakeOwnership"))
            {
                security.Owner = ntfsAccountName;
                //NTFSSecurity.TakeOwnership(path, ntfsAccountName);
                Console.WriteLine("As admin? {0}", NTFSSecurity.IsAdministrator());
                Console.WriteLine("Object owner? {0}", security.Owner);
                Console.WriteLine("Param runner? {0}", argumnet);
            }

            security.Owner = ntfsAccountName;
        }
    }
}

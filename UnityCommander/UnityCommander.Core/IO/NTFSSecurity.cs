using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UnityCommander.Core.IO
{
    public class NTFSSecurity
    {
        /// <summary>
        /// Contains path to file/folder.
        /// </summary>
        private static string _path;
        /// <summary>
        /// Contains reference for manage process.
        /// </summary>
        private static Process _processRef;
        /// <summary>
        /// Gets or sets owner for current object.
        /// </summary>
        public string Owner 
        {
            get 
            { 
                DirectoryInfo directory = new DirectoryInfo(_path);
                DirectorySecurity directorySecurity = directory.GetAccessControl();
                var identy = directorySecurity.GetOwner(typeof(NTAccount));
                return identy.Value;
            }
            set
            {
                if (_processRef != null)
                    _processRef.Kill();
                // If the program already has elevated rights
                // there is no need to start a new process. 
                if (IsAdministrator())
                    TakeOwnership(value);
                else
                    StartProcessAsAdmin("-TakeOwnership");
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NTFSSecurity"/> class.
        /// </summary>
        /// <param name="path"></param>
        public NTFSSecurity(string path)
        {
            _path = path;
        }
        /// <summary>
        /// Gets the ntfs account of the current object owner.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <returns> The reference to manage the ntfs account. </returns> 
        public static IdentityReference GetOwner(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectorySecurity directorySecurity = directory.GetAccessControl();
            return directorySecurity.GetOwner(typeof(NTAccount));
        }
        /// <summary>
        /// The method sets the specified user as the new owner of this object.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <param name="ntfsAccountName"> The ntfs account of the new owner. </param>
        public static void TakeOwnership(string ntfsAccountName)
        {
            SecurityIdentifier identifier = ConvertStringToSID(ntfsAccountName);
            DirectoryInfo directory = new DirectoryInfo(_path);
            DirectorySecurity directorySecurity = directory.GetAccessControl();              
            directorySecurity.SetOwner(identifier);
            directory.SetAccessControl(directorySecurity);
        }
        /// <summary>
        /// Changes the permissions for an existing ntfs account.
        /// May need administrator rights for perform this operation.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <param name="account"> NTFS Acoount name. For example "Users". </param>
        /// <param name="rights"> Access right set that be have ntfs account. </param>
        /// <param name="controlType"> Deny or allow access to a protected object. </param>
        public static void ChangeAccessRight(string path, string account, FileSystemRights rights, AccessControlType controlType)
        {
            SecurityIdentifier identifier = ConvertStringToSID(account);
            FileInfo info = new FileInfo(path);
            FileSecurity fileSecurity = info.GetAccessControl();
            fileSecurity.SetAccessRule(new FileSystemAccessRule(identifier, rights, controlType));
            info.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// Gets all ntfs accounts of this file/folder.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <returns> The collection with all required NTFS account details. </returns>
        public static List<FileSystemAccessRule> GetNTAccounts(string path)
        {
            List<FileSystemAccessRule> accountCal = new List<FileSystemAccessRule>();
            FileInfo info = new FileInfo(path);
            FileSecurity accessControl = info.GetAccessControl();
            var ruleCollection = accessControl.GetAccessRules(true, true, typeof(NTAccount));
            // Adding all ntfs accounts of the file or folder.
            foreach (var account in ruleCollection)
                accountCal.Add((FileSystemAccessRule)account);
            return accountCal;
        }
        /// <summary>
        /// Adds new ntfs account for file or folder. 
        /// May need administrator rights for perform this operation.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <param name="account"> NTFS Acoount name. For example "Users". </param>
        /// <param name="rights"> Access right set that be have ntfs account. </param>
        /// <param name="controlType"> Deny or allow access to a protected object. </param>
        public static void AddNTAccount(string path, string account, FileSystemRights rights, AccessControlType controlType)
        {
            SecurityIdentifier identifier = ConvertStringToSID(account);
            FileInfo info = new FileInfo(path);
            FileSecurity fileSecurity = info.GetAccessControl();        
            fileSecurity.AddAccessRule(new FileSystemAccessRule(identifier, rights, controlType));
            info.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// Flush and add new access right to file.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <param name="accessRules"> Users access rights list to file. </param>
        public static void AddAccessRuleList(string path, List<FileSystemAccessRule> accessRules)
        {
            // Get file info
            FileInfo info = new FileInfo(path);
            // Get security access
            FileSecurity fileSecurity = info.GetAccessControl();
            //remove any inherited access
            fileSecurity.SetAccessRuleProtection(true, false);

            // Add current user with full control.
            foreach (var rule in accessRules)
            {
                fileSecurity.AddAccessRule(rule);
            }
            // Flush security access.
            info.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// Remove ntfs account for file or folder. 
        /// May need administrator rights for perform this operation.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <param name="account"> NTFS Acoount name. For example "Users". </param>
        /// <param name="rights"> Access right set that be have ntfs account. </param>
        /// <param name="controlType"> Deny or allow access to a protected object. </param>
        public void RemoveNTAccount(string account, FileSystemRights rights, AccessControlType controlType)
        {
            SecurityIdentifier identifier = ConvertStringToSID(account);
            FileInfo info = new FileInfo(_path);
            FileSecurity fileSecurity = info.GetAccessControl();
            fileSecurity.RemoveAccessRule(new FileSystemAccessRule(identifier, rights, controlType));
            info.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// Convert username to SID string.
        /// </summary>
        /// <param name="identifierName"> Username, for example "Administrators" </param>
        /// <returns> SID string. </returns>
        private static SecurityIdentifier ConvertStringToSID(string identifierName)
        {
            NTAccount owner = new NTAccount(identifierName);
            return (SecurityIdentifier)owner.Translate(typeof(SecurityIdentifier));
        }
        /// <summary>
        /// Checks require whether of the program elevated rights.
        /// </summary>
        /// <returns> Returns true if the program has admin rights. </returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity == null) return false;
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// Run process as admin, some operation requires higher privileges.
        /// </summary>
        /// <param name="arguments"> Arguments identificate what method requires elevated rights. </param>
        static void StartProcessAsAdmin(string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = AppDomain.CurrentDomain.BaseDirectory + "UnityCommander.Test.exe";
                psi.Arguments = arguments;
                psi.UseShellExecute = true;
                psi.Verb = "runas";
                _processRef = Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

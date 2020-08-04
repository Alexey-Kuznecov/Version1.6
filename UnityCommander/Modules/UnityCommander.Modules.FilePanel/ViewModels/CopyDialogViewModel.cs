using System;
using System.Collections.Generic;
using Prism.Mvvm;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class CopyDialogViewModel : BindableBase
    {
        public CopyDialogViewModel()
        {

        }

        public string Source { get; set; }
        public string Target { get; set; }
        public bool Md5Enable { get; set; }
        public List<string> ExtensionInclude { get; set; }
        public List<string> ExtensionExclude { get; set; }
    }
}

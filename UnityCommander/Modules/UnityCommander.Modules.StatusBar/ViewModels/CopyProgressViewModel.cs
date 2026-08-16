
using System.Collections.ObjectModel;
using UnityCommander.Core.IO;

namespace UnityCommander.Modules.StatusBar.ViewModels
{
    public class CopyProgressViewModel : BindableBase
    {
        public ReadOnlyObservableCollection<CopyOperationViewModel> Operations { get; }

        public CopyProgressViewModel(ICopyOperationService operationService)
        {
            Operations = ((CopyOperationService)operationService).Operations;
        }
    }
}

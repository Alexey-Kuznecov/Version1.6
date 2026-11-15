
using UnityCommander.Diagnostics.Workbench.ViewModels.Base;

namespace UnityCommander.Diagnostics.Workbench.ViewModels.Reporting
{
    public sealed class DiagnosticComponentFilterItem : ObservableObject
    {
        private bool _isSelected;

        public DiagnosticComponentFilterItem(string name)
        {
            Name = name;
            _isSelected = true;
        }

        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}

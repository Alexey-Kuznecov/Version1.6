
using Prism.Mvvm;
using System.Collections.ObjectModel;
using UnityCommander.Autocomplete.Completion;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleState : BindableBase
    {
        private readonly ConsoleAutocompleteProcessor _autocompleteProcessor;

        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetProperty(ref _inputText, value))
                {
                    //if (!SuppressCompletionUpdate)
                    //    _autocompleteProcessor.UpdateCompletions(this);
                }
            }
        }

        private int _caretIndex;

        public int CaretIndex
        {
            get => _caretIndex;
            set
            {
                if (SetProperty(ref _caretIndex, value))
                {
                    if (!SuppressCompletionUpdate)
                        _autocompleteProcessor.UpdateCompletions(this);
                }
            }
        }

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public ObservableCollection<CompletionItem> Completions { get; }
            = new();

        public bool IsCompletionVisible =>
            Completions.Count > 0;

        public bool SuppressCompletionUpdate { get; set; }


        public ConsoleState(ConsoleAutocompleteProcessor autocompleteProcessor)
        {
            _autocompleteProcessor = autocompleteProcessor;
            InputText = string.Empty;
            CaretIndex = 1;
        }
    }
}


using System.ComponentModel;
using System.Windows.Input;
using UnityCommander.Controls.Ribbon2.Abstraction;
using UnityCommander.Controls.Ribbon2.Models;

namespace UnityCommander.Controls.Ribbon2.Mvvm
{
    public class RibbonViewModel : INotifyPropertyChanged
    {
        public RibbonModel Model { get; }
        public ICommand AddToolCommand { get; }
        public ICommand RemoveToolCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private readonly ICommandRegistry commandRegistry;
        private readonly ISerializer serializer;
        private readonly IUndoService undoService;

        public event PropertyChangedEventHandler PropertyChanged;

        public RibbonViewModel(RibbonModel model, ICommandRegistry registry, ISerializer serializer, IUndoService undoService)
        {
            Model = model;
            commandRegistry = registry;
            this.serializer = serializer;
            this.undoService = undoService;
            // init commands...
        }

        // методы для редактирования модели: AddTool, MoveTool, ChangeLayout ...
    }
}

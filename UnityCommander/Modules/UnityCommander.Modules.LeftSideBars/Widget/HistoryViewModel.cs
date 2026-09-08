
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using UnityCommander.Abstractions.History;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.LeftSideBars.Widget.Actions;
using UnityCommander.Modules.LeftSideBars.Widget.Models;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public sealed class HistoryViewModel : BindableBase
    {
        private PanelActionExecutor _executor;

        private HistoryInfoModel _selectedFolder;

        private const int DisplayLimit = 6;

        private readonly IUserNavigationHistory _history;

        public ObservableCollection<HistoryInfoModel> History { get; } = [];

        public HistoryViewModel(
            IUserNavigationHistory history,
            ActiveTabContext activeTabContext,
            PanelActionExecutor executor)
        {
            _executor = executor;
            _history = history;

            activeTabContext.SubscribeCurrentPath(
                OnCurrentPathChanged);

            LoadHistory();
        }

        public HistoryInfoModel SelectedHistoryFolder
        {
            get => _selectedFolder;
            set
            {
                if (SetProperty(ref _selectedFolder, value))
                {
                    if (string.IsNullOrWhiteSpace(_selectedFolder?.Path))
                        return;

                    _executor.Navigate(_selectedFolder.Path);
                }
            }
        }

        private void LoadHistory()
        {
            foreach (var path in _history.Paths.Take(DisplayLimit))
            {
                History.Add(CreateInfo(path));
            }
        }

        private void OnCurrentPathChanged(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            _history.Add(path);

            History.Clear();

            foreach (var item in _history.Paths.Take(DisplayLimit))
                History.Add(CreateInfo(item));
        }

        private static HistoryInfoModel CreateInfo(string path)
        {
            var name = System.IO.Path.GetFileName(
                path.TrimEnd(
                    System.IO.Path.DirectorySeparatorChar,
                    System.IO.Path.AltDirectorySeparatorChar));

            if (string.IsNullOrEmpty(name))
                name = path;

            return new HistoryInfoModel
            {
                Name = name,
                Path = path,
                IconKey = "folder"
            };
        }
    }
}

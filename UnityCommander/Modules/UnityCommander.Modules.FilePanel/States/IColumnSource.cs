
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.States
{
    public interface IColumnSource<T> where T : BaseDirectory
    {
        ObservableCollection<T> Items { get; }
    }
}
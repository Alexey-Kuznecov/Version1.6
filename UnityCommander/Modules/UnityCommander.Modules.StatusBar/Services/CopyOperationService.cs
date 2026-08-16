
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using UnityCommander.Abstractions.IO;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Modules.StatusBar.ViewModels;

namespace UnityCommander.Modules.StatusBar
{
    public class CopyOperationService : ICopyOperationService
    {
        private readonly ConcurrentDictionary<Guid, CopyManager> _managers = new();

        private readonly ObservableCollection<CopyOperationViewModel> _operations = new();

        public ReadOnlyObservableCollection<CopyOperationViewModel> Operations { get; }

        public CopyOperationService()
        {
            Operations = new ReadOnlyObservableCollection<CopyOperationViewModel>(_operations);
        }

        public IReadOnlyCollection<CopyManager> Managers => (IReadOnlyCollection<CopyManager>)_managers.Values;

        public CopyManager Get(Guid operationId)
        {
            return _managers.TryGetValue(operationId, out var manager)
                ? manager
                : throw new KeyNotFoundException(
                    $"Copy manager with operation id '{operationId}' was not found.");
        }

        public bool TryGet(Guid operationId, out CopyManager manager)
        {
            return _managers.TryGetValue(operationId, out manager!);
        }

        public void Register(CopyManager manager, IOperationProgressService progressService)
        {
            if (!_managers.TryAdd(manager.Id, manager))
                throw new InvalidOperationException();

            _operations.Add(new CopyOperationViewModel(manager, progressService));
        }

        public bool Unregister(Guid operationId)
        {
            if (!_managers.TryRemove(operationId, out var manager))
                return false;

            var vm = _operations.FirstOrDefault(x => x.Id == operationId);

            if (vm != null)
                _operations.Remove(vm);

            return true;
        }
    }
}


using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.Controls;
using UnityCommander.Services.Interfaces;
using UnityCommander.UI.Overlay;
using UnityCommander.UI.Visual;

namespace UnityCommander.Modules.FilePanel.Services
{
    public sealed class RenameManager : IRenameManager
    {
        private readonly IRenameService _renameService;
        private readonly ISelectionService _selectionService;
        private readonly IVisualElementRegistry _visualElements;
        private readonly IOverlayService _overlayService;

        private string? _sourcePath;

        public bool IsActive => _sourcePath is not null;

        public RenameManager(
            IRenameService renameService,
            ISelectionService selectionService,
            IVisualElementRegistry visualElements,
            IOverlayService overlayService)
        {
            _renameService = renameService;
            _selectionService = selectionService;
            _visualElements = visualElements;
            _overlayService = overlayService;
        }

        public void Start()
        {
            var active = _selectionService.GetActive();

            if (active.SelectedItems.Count != 1)
                return;

            Start((BaseDirectory)active.SelectedItems.First());
        }

        public void Start(BaseDirectory item)
        {
            var element = _visualElements.GetElement(item);

            if (element is null)
                return;

            _sourcePath = item.Path;

            var overlay = new RenameOverlay();

            overlay.RenameRequested += async newName =>
            {
                item.Name = newName;
                await CommitAsync(newName);
            };

            _overlayService.Show(element, overlay);

            if (item is FileModel file)
            {
                overlay.BeginEdit(
                    file.Name + file.Extension,
                    isFile: true);

                return;
            }

            overlay.BeginEdit(
                item.Name,
                isFile: false);
        }

        public async Task CommitAsync(string newName)
        {
            if (_sourcePath is null)
                return;

            await _renameService.RenameAsync(
                _sourcePath,
                newName);

            _sourcePath = null;
        }

        public void Cancel()
        {
            _sourcePath = null;
        }
    }
}

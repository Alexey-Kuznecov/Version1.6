
using UnityCommander.Abstractions.Panels;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using UnityCommander.Common.Panels;
    using UnityCommander.Preview.Abstraction.Interfaces;
    using UnityCommander.Preview.Abstraction.Models;
    using UnityCommander.Preview.Wpf;

    // Добавить using из существующих проектов, где объявлены:
    // ISelectionService, ISelectionManager, IPanelRegistry,
    // ActiveTabChangedEvent и IDirectoryItem.

    public sealed class PreviewViewModel : BindableBase, IDisposable
    {
        private readonly IPreviewService _previewService;
        private readonly IPreviewRegistry _previewRegistry;
        private readonly ISelectionService _selectionService;
        private readonly IPanelRegistry _panelRegistry;

        private readonly ActiveTabContext _activeTabContext;

        private IDictionary<string, string>? _pendingStates;

        private ISelectionManager? _selectionManager;

        private CancellationTokenSource? _previewCancellation;
        private int _requestVersion;

        private PreviewResult? _previewResult;
        private IPreviewPresenter? _currentPresenter;
        private object? _presenterView;

        private string? _requestedPath;

        private string? _filePath;
        private string? _fileName;
        private string? _errorMessage;

        private bool _isLoading;
        private bool _isUnsupported;

        private PreviewKind? _kind;
        private PreviewMetadata? _metadata;
        private PreviewCapabilities _capabilities;

        private object? _presenterContent;

        private bool _disposed;

        public string? FilePath
        {
            get => _filePath;
            private set => SetProperty(ref _filePath, value);
        }

        public string? FileName
        {
            get => _fileName;
            private set => SetProperty(ref _fileName, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public bool IsUnsupported
        {
            get => _isUnsupported;
            private set => SetProperty(ref _isUnsupported, value);
        }

        public PreviewKind? Kind
        {
            get => _kind;
            private set => SetProperty(ref _kind, value);
        }

        public PreviewMetadata? Metadata
        {
            get => _metadata;
            private set => SetProperty(ref _metadata, value);
        }

        public PreviewCapabilities Capabilities
        {
            get => _capabilities;
            private set => SetProperty(ref _capabilities, value);
        }

        public object? PresenterContent
        {
            get => _presenterContent;
            private set => SetProperty(ref _presenterContent, value);
        }

        public PreviewViewModel(
            IPreviewService previewService,
            IPreviewRegistry previewRegistry,
            ISelectionService selectionService,
            IPanelRegistry panelRegistry,
            ActiveTabContext activeTabContext)
        {
            _previewService = previewService;
            _previewRegistry = previewRegistry;
            _selectionService = selectionService;
            _panelRegistry = panelRegistry;

            _panelRegistry.ActiveTabChanged += OnActiveTabChanged;

            _activeTabContext = activeTabContext;
                    _activeTabContext.SubscribeCurrentPath(OnCurrentPathChanged);

            BindSelection(_selectionService.GetActive());
        }

        #region Panel and selection

        private void OnActiveTabChanged(ActiveTabChangedEvent args)
        {
            BindSelection(_selectionService.Get(args.TabId));
        }

        private void BindSelection(ISelectionManager? manager)
        {
            if (ReferenceEquals(_selectionManager, manager))
                return;

            if (_selectionManager != null)
                _selectionManager.SelectionChanged -= OnSelectionChanged;

            _selectionManager = manager;

            if (_selectionManager != null)
                _selectionManager.SelectionChanged += OnSelectionChanged;

            OnSelectionChanged();
        }

        private void OnPreviewSourceChanged()
        {
            _ = RefreshPreviewAsync();
        }

        private void OnCurrentPathChanged(string path)
        {
            OnPreviewSourceChanged();
        }

        private void OnSelectionChanged()
        {
            OnPreviewSourceChanged();
        }

        private string? GetSelectedPath()
        {
            if (_selectionManager == null)
                return null;

            // В первую очередь используем элемент в фокусе.
            if (_selectionManager.FocusedItem is IDirectoryItem focused)
                return focused.Path;

            // Если фокуса нет, используем единственный выбранный элемент.
            if (_selectionManager.SelectedItems.Count == 1)
            {
                foreach (var selected in _selectionManager.SelectedItems)
                {
                    if (selected is IDirectoryItem item)
                        return item.Path;
                }
            }

            return null;
        }

        #endregion

        #region Preview

        private Task RefreshPreviewAsync()
        {
            if (_disposed)
                return Task.CompletedTask;

           var path = GetPreviewSourcePath();

            // Повторное уведомление для того же файла
            // не должно запускать повторную обработку.
            if (string.Equals(
                _requestedPath,
                path,
                StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _requestedPath = path;

            var version = ++_requestVersion;

            CancelCurrentRequest();
            ClearCurrentPreview();

            FilePath = path;
            FileName = path == null ? null : Path.GetFileName(path);

            Metadata = null;
            Kind = null;
            Capabilities = PreviewCapabilities.None;

            ErrorMessage = null;
            IsUnsupported = false;
            IsLoading = false;

            if (string.IsNullOrWhiteSpace(path))
                return Task.CompletedTask;

            var cancellation = new CancellationTokenSource();

            _previewCancellation = cancellation;

            IsLoading = true;

            return LoadPreviewAsync(path, version, cancellation);
        }

        private async Task LoadPreviewAsync(
            string path,
            int version,
            CancellationTokenSource cancellation)
        {
            PreviewResult? result = null;

            IPreviewPresenter? presenter = null;
            object? presenterView = null;

            try
            {
                result = await _previewService.PreviewAsync(
                    new PreviewRequest(path),
                    cancellation.Token);

                if (!IsCurrentRequest(version, cancellation.Token))
                    return;

                if (result == null)
                {
                    IsUnsupported = true;
                    return;
                }

                // Используем существующую модель результата.
                Metadata = result.Metadata;
                Kind = result.Content.Kind;
                Capabilities = result.Capabilities;

                // Выбираем Presenter из существующего реестра.
                presenter = _previewRegistry.FindPresenter(result);

                if (presenter == null)
                {
                    IsUnsupported = true;
                    return;
                }

                presenterView = presenter.CreateView();

                presenter.Present(presenterView, result);

                if (!IsCurrentRequest(version, cancellation.Token))
                {
                    ClearPresenter(presenter, presenterView);
                    presenterView = null;
                    return;
                }

                // Сохраняем весь PreviewResult.
                // Он будет освобождён при смене файла или закрытии инструмента.
                _previewResult = result;
                _currentPresenter = presenter;
                _presenterView = presenterView;

                PresenterContent = presenterView;

                TryRestorePresenterState();

                // Владение результатом передано ViewModel.
                result = null;
                presenter = null;
                presenterView = null;
            }
            catch (OperationCanceledException)
                when (cancellation.IsCancellationRequested)
            {
                // Предыдущий запрос больше не актуален.
            }
            catch (Exception ex)
            {
                if (IsCurrentRequest(version, cancellation.Token))
                    ErrorMessage = ex.Message;
            }
            finally
            {
                if (presenter != null && presenterView != null)
                    ClearPresenter(presenter, presenterView);

                result?.Dispose();

                if (ReferenceEquals(_previewCancellation, cancellation))
                    _previewCancellation = null;

                if (IsCurrentRequest(version, cancellation.Token))
                    IsLoading = false;

                cancellation.Dispose();
            }
        }

        private bool IsCurrentRequest(
            int version,
            CancellationToken cancellationToken)
        {
            return !_disposed
                && version == _requestVersion
                && !cancellationToken.IsCancellationRequested;
        }

        private void CancelCurrentRequest()
        {
            var cancellation = _previewCancellation;

            _previewCancellation = null;

            cancellation?.Cancel();
        }

        private void ClearCurrentPreview()
        {
            var result = _previewResult;
            var presenter = _currentPresenter;
            var presenterView = _presenterView;

            _previewResult = null;
            _currentPresenter = null;
            _presenterView = null;

            PresenterContent = null;

            try
            {
                if (presenter != null && presenterView != null)
                    presenter.Clear(presenterView);
            }
            finally
            {
                result?.Dispose();
            }
        }

        private static void ClearPresenter(
            IPreviewPresenter presenter,
            object view)
        {
            try
            {
                presenter.Clear(view);
            }
            catch
            {
                // Не скрываем исходную ошибку обработки.
            }
        }

        #endregion

        public void CaptureState(IDictionary<string, string> states)
        {
            if (_presenterView is not FrameworkElement view ||
                view.DataContext is not IPreviewStateProvider stateProvider)
                return;

            var state = stateProvider.CaptureState();

            states[stateProvider.StateId] =
                JsonSerializer.Serialize(
                    state,
                    state.GetType(),
                    new JsonSerializerOptions());
        }

        public void RestoreState(IDictionary<string, string> states)
        {
            _pendingStates = new Dictionary<string, string>(states);

            TryRestorePresenterState();
        }

        private void TryRestorePresenterState()
        {
            if (_pendingStates == null)
                return;

            if (_presenterView is not FrameworkElement view ||
                view.DataContext is not IPreviewStateProvider stateProvider)
                return;

            if (!_pendingStates.TryGetValue(stateProvider.StateId, out var json))
                return;

            stateProvider.RestoreState(json);

            _pendingStates.Remove(stateProvider.StateId);

            if (_pendingStates.Count == 0)
                _pendingStates = null;
        }

        private string? GetPreviewSourcePath()
        {
            return GetSelectedPath() ?? _activeTabContext.CurrentPath;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _requestVersion++;

            _activeTabContext.UnsubscribeCurrentPath(OnCurrentPathChanged);

            _panelRegistry.ActiveTabChanged -= OnActiveTabChanged;

            if (_selectionManager != null)
            {
                _selectionManager.SelectionChanged -= OnSelectionChanged;
                _selectionManager = null;
            }

            CancelCurrentRequest();
            ClearCurrentPreview();
        }
    }
}

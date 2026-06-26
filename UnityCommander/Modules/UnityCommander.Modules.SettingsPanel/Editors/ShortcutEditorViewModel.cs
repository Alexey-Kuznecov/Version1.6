
using Prism.Commands;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.Services;
using UnityCommander.Mvvm.Base;
using UnityCommander.Settings.Abstactions;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Editors
{
    public sealed class ShortcutEditorViewModel : PropertiesChanged
    {
        public ShortcutDefinition Definition { get; set; }

        private ShortcutOverride _value;

        private IInputCaptureManager _captureManager;

        private readonly IShortcutOverrideStore _shortcutStore;

        private readonly JsonShortcutOverrideStorage _shotcutStorage;
        
        private readonly IShortcutMapProvider _shortcutMap;

        public ShortcutEditorViewModel(
            IInputCaptureManager captureManager,
            ISettingsService settingsService,
            IShortcutOverrideStore shortcutStore,
            IShortcutMapProvider shortcutMap,
            JsonShortcutOverrideStorage shotcutStorage)
        {
            _captureManager = captureManager;
            _shortcutMap = shortcutMap;
            _shortcutStore = shortcutStore;
            _shotcutStorage = shotcutStorage;

            BeginCaptureCommand =
                new DelegateCommand(BeginCapture);
        }

        public DelegateCommand BeginCaptureCommand { get; }

        public ShortcutOverride Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }

        private ShortcutKey CurrentKey =>
            Value?.Key ?? Definition.Key;

        private ShortcutModifiers CurrentModifiers =>
            Value?.Modifiers ?? Definition.Modifiers;

        public string Display =>
            $"{(CurrentModifiers == ShortcutModifiers.None ? "" : $"{CurrentModifiers}+")}{CurrentKey}";

        public string Description => Definition.Description;

        private void BeginCapture()
        {
            if (IsRecording)
                return;

            IsRecording = true;

            _captureManager.Push(
                new ShortcutCaptureContext(
                    result =>
                    {
                        SetShortcut(result);

                        IsRecording = false;

                        OnPropertyChanged(nameof(Display));
                    },
                    () =>
                    {
                        IsRecording = false;

                        _captureManager.Pop();
                    }));
        }

        private void SetShortcut(InputEvent input)
        {
            var newShortcut = new ShortcutOverride()
            {
                CommandId = Definition.CommandId,
                Key = input.Key,
                Modifiers = input.Modifiers,
            };

            Value = newShortcut;

            if (_shortcutStore.TryGet(newShortcut?.CommandId, out var @override))
            {
                if (newShortcut.Key == Definition.Key &&
                    newShortcut.Modifiers == Definition.Modifiers)
                {
                    _shortcutStore.Remove(Definition.CommandId);
                }
                else
                {
                    _shortcutStore.Set(newShortcut);
                }
            }
            else
                _shortcutStore.TrySet(newShortcut);

            _shotcutStorage.Save(_shortcutStore.GetSnapshot());
            _shortcutMap.Rebuild();
        }
    }
}

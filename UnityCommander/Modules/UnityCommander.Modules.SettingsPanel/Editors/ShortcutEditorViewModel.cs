
using Prism.Commands;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.Modules.SettingsPanel.Services;
using UnityCommander.Mvvm.Base;
using UnityCommander.Settings.Abstactions;
using UnityCommander.Settings.Core;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Editors
{
    public sealed class ShortcutEditorViewModel : PropertiesChanged
    {
        public SettingDefinition Definition { get; set; }

        private ShortcutOverride _value;

        private IInputCaptureManager _captureManager;

        private ISettingsService _settingsService;

        public ShortcutEditorViewModel(
            IInputCaptureManager captureManager,
            ISettingsService settingsService)
        {
            _captureManager = captureManager;
            _settingsService = settingsService;

            BeginCaptureCommand =
                new DelegateCommand(BeginCapture);
        }

        public DelegateCommand BeginCaptureCommand { get; }

        public ShortcutOverride Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
            }
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }

        public string Display =>
            $"{Value.Modifiers}+{Value.Key}";

        public string Description => Definition.Description;

        private void BeginCapture()
        {
            if (IsRecording)
                return;

            IsRecording = true;

            _captureManager.Push(
                new ShortcutCaptureContext(
                    shortcut =>
                    {
                        SetShortcut(shortcut);

                        IsRecording = false;

                        OnPropertyChanged(nameof(Display));
                    },
                    () =>
                    {
                        IsRecording = false;

                        _captureManager.Pop();
                    }));
        }

        private void SetShortcut(ShortcutOverride newShortcut)
        {
            Value = newShortcut;

            _settingsService.Set(Definition, newShortcut);
        }
    }
}

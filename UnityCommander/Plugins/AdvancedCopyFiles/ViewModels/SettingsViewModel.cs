
using AdvancedCopyFiles.Core;
using CommandSystem.Gui.MVVM;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Copying.Sessions;

namespace AdvancedCopyFiles.ViewModels
{
    public class SettingsViewModel : ObservableObject, IInitializable
    {
        private CopySessionManager _copySessionManager;
        private ICopySettingsBuilder _builder;

        private string _sourcePath = string.Empty;
        private string _destinationPath = string.Empty;

        private int _maxConcurrentTasks = 5;
        private int _bufferSize = 81920;
        private bool _allowEmptyDirectories;
        private bool _useMultiThreading;
        private bool _overwriteExistingFiles;
        private bool _сopyAllToOneFolder;
        private bool _flattenStructure;

        public SettingsViewModel(IMessageBus message, ICopySettingsBuilder copySettings, CopySessionManager manager)
        {
            _builder = copySettings;
            _copySessionManager = manager;
         
            message.Subscribe<StartRequestedMessage>(OnStartRequested);
        }

        private ValueTask OnStartRequested(StartRequestedMessage message)
        {
            var ctx = message.Context;

            if (ctx == null)
                return ValueTask.CompletedTask;

            ctx.Source = _sourcePath;
            ctx.Destination = _destinationPath;

            ctx.Session = _copySessionManager.CreateSession(
                SourcePath,
                DestinationPath);

            ctx.Settings = _builder.Build(this, ctx.Session);

            return ValueTask.CompletedTask;
        }

        public void Initialize(object parameter)
        {
            if (parameter is not FileOperationRequest request)
                return;

            SourcePath = request.Sources[0];
            DestinationPath = request.Target;
       
            UseMultiThreading = false;
            MaxConcurrentTasks = 2;
        }

        public int MaxConcurrentTasks
        {
            get => _maxConcurrentTasks;
            set => SetProperty(ref _maxConcurrentTasks, value);
        }

        public int BufferSize
        {
            get => _bufferSize;
            set => SetProperty(ref _bufferSize, value);
        }

        public bool AllowEmptyDirectories
        {
            get => _allowEmptyDirectories;
            set => SetProperty(ref _allowEmptyDirectories, value);
        }

        public bool UseMultiThreading
        {
            get => _useMultiThreading;
            set => SetProperty(ref _useMultiThreading, value);
        }

        public bool OverwriteExistingFiles
        {
            get => _overwriteExistingFiles;
            set => SetProperty(ref _overwriteExistingFiles, value);
        }
        public bool FlattenStructure
        {
            get => _flattenStructure;
            set => SetProperty(ref _flattenStructure, value);
        }
        public bool CopyAllToOneFolder
        {
            get => _сopyAllToOneFolder;
            set => SetProperty(ref _сopyAllToOneFolder, value);
        }
        public string SourcePath
        {
            get => _sourcePath;
            set => SetProperty(ref _sourcePath, value);
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set => SetProperty(ref _destinationPath, value);
        }
    }
}

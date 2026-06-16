
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyProcessViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//  The class is a view model for dialog window of the copy files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.ViewModels.Dialogs
{
    using Prism.Commands;
    using Prism.Mvvm;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using UnityCommander.Abstractions.Dialog;
    using UnityCommander.Core.IO;
    using UnityCommander.Operation;

    public class CopyProcessViewModel : BindableBase, IDialogAware<CopyDialogResult>
    {
        private readonly CopyOperationController copyController;
        
        private double exactPercent;
        
        private int currentPercent;
        
        public DelegateCommand StopCommand { get; }
        
        public DelegateCommand CancelCommand { get; }
       
        public DelegateCommand ResumeCommand { get; }

        private ObservableCollection<CopyInfoModel> copyReport;

        private ObservableCollection<CopyInfoModel> skippedFiles;
       
        public ObservableCollection<CopyInfoModel> CopyReport
        {
            get => copyReport;
            set => SetProperty(ref copyReport, value);
        }

        public ObservableCollection<CopyInfoModel> SkippedFiles
        {
            get => skippedFiles;
            set => SetProperty(ref skippedFiles, value);
        }
     
        public int CurrentPercent
        {
            get => currentPercent;
            set => SetProperty(ref currentPercent, value);
        }

     
        public double ExactPercent
        {
            get => exactPercent;
            set => SetProperty(ref exactPercent, value);
        }

        private string averageSpeed;
        public string AverageSpeed
        {
            get => averageSpeed;
            set => SetProperty(ref averageSpeed, value);
        }

        private string remainder;
        public string Remainder
        {
            get => remainder;
            set => SetProperty(ref remainder, value);
        }

        private string timeLeft;
        public string TimeLeft
        {
            get => timeLeft;
            set => SetProperty(ref timeLeft, value);
        }

        public CopyDialogResult Result { get; set; }

        public Action RequestClose { get; set; }

        public CopyProcessViewModel(CopyOperationController controller)
        {
            copyController = controller;

            StopCommand = new DelegateCommand(copyController.Pause);
            CancelCommand = new DelegateCommand(copyController.Cancel);
            ResumeCommand = new DelegateCommand(copyController.Resume);

            copyController.ProgressChanged += OnProgressChanged;
            copyController.FileCopied += OnFileCopied;
            copyController.FileSkipped += OnFileSkipped;
            copyController.Completed += OnCopyCompleted;
        }

        private void OnProgressChanged(ProgressModel progress)
        {
            CurrentPercent = progress.Percent;
            ExactPercent = progress.ExactPercent;
            AverageSpeed = progress.Speed;
            Remainder = progress.Remainder;
            TimeLeft = progress.TimeLeft;
        }

        private void OnFileCopied(CopyInfoModel info)
            => CopyReport.Add(info);
        

        private void OnFileSkipped(CopyInfoModel info)
           => SkippedFiles.Add(info);
        
        private void OnCopyCompleted(CopyOperationResult copyOperation)
        {
            copyController.ProgressChanged -= OnProgressChanged;
            copyController.FileCopied -= OnFileCopied;
            copyController.FileSkipped -= OnFileSkipped;
            copyController.Completed -= OnCopyCompleted;

            Application.Current.Dispatcher.Invoke(() =>
            {
                RequestClose?.Invoke();
            });
        }

        public void OnDialogOpened(object parameter) { }

        public bool CanCloseDialog()
            => true;

        public void OnDialogClosed() { }
    }
}

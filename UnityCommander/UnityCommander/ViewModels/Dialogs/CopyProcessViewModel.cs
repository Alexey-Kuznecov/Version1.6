
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
    using System;
    using System.Collections.ObjectModel;
    using Prism.Commands;
    using Prism.Mvvm;
    using UnityCommander.Core.IO;
    using UnityCommander.Operation;

    public class CopyProcessViewModel : BindableBase, IDisposable
    {
        private readonly CopyOperationController copyController;

        public DelegateCommand StopCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ResumeCommand { get; }

        private ObservableCollection<CopyInfoModel> copyReport;
        public ObservableCollection<CopyInfoModel> CopyReport
        {
            get => copyReport;
            set => SetProperty(ref copyReport, value);
        }

        private ObservableCollection<CopyInfoModel> skippedFiles;
        public ObservableCollection<CopyInfoModel> SkippedFiles
        {
            get => skippedFiles;
            set => SetProperty(ref skippedFiles, value);
        }

        private int currentPercent;
        public int CurrentPercent
        {
            get => currentPercent;
            set => SetProperty(ref currentPercent, value);
        }

        private double exactPercent;
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
        {
            CopyReport.Add(info);
        }

        private void OnFileSkipped(CopyInfoModel info)
        {
            SkippedFiles.Add(info);
        }

        private void OnCopyCompleted()
        {
            // тут можно триггерить глобальные команды
        }

        public void Dispose()
        {
            copyController.ProgressChanged -= OnProgressChanged;
            copyController.FileCopied -= OnFileCopied;
            copyController.FileSkipped -= OnFileSkipped;
            copyController.Completed -= OnCopyCompleted;
        }
    }
}

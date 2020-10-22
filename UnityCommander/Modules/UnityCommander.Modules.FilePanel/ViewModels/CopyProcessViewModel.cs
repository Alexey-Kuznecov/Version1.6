
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyProcessViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//  The class is a view model for dialog window of the copy files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using UnityCommander.Core;
    using UnityCommander.Core.Helper;
    using UnityCommander.Core.IO;
    using UnityCommander.Modules.FilePanel.Commands;

    /// <summary>
    /// The class is a view model for dialog window of the copy files.
    /// </summary>
    public class CopyProcessViewModel : BindableBase
    {
        #region Declaration Fields

        /// <summary>
        /// The invoker class instance.
        /// </summary>
        private readonly CopyFileInvoker invoker;

        /// <summary>
        /// Contains the current progress bar for copying a file.
        /// </summary>
        private int currentPercent;

        /// <summary>
        /// Contains the min value of the progress bar.
        /// </summary>
        private int startBar;

        /// <summary>
        /// The report data about copying files.
        /// </summary>
        private ObservableCollection<CopyInfoModel> copyReport;

        /// <summary>
        /// The list copy errors.
        /// </summary>
        private ObservableCollection<CopyInfoModel> skippedFile;

        /// <summary>
        /// The time left.
        /// </summary>
        private string timeLeft;

        /// <summary>
        /// The average speed.
        /// </summary>
        private string averageSpeed;

        /// <summary>
        /// The remainder.
        /// </summary>
        private string remainder;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProcessViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyProcessViewModel(IEventAggregator viewModelMessage)
        {
            this.invoker = new CopyFileInvoker(this.invoker);
            viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetupCopyFiles);
            this.StopCommand = new DelegateCommand(this.CopySuspend);
            this.CancelCommand = new DelegateCommand(this.CopyCancel);
            this.ResumeCommand = new DelegateCommand(this.CopyResume);
        }

        #endregion

        #region Declaration Properties

        /// <summary>
        /// Gets or sets the report data for copied files.
        /// </summary>
        public ObservableCollection<CopyInfoModel> CopyReport
        {
            get => this.copyReport;
            set => this.SetProperty(ref this.copyReport, value);
        }

        /// <summary>
        /// Gets or sets the report data for copied files.
        /// </summary>
        public ObservableCollection<CopyInfoModel> SkippedFile
        {
            get => this.skippedFile;
            set => this.SetProperty(ref this.skippedFile, value);
        }

        /// <summary>
        /// Gets command to pause the copy thread.
        /// </summary>
        public DelegateCommand StopCommand { get; }
        
        /// <summary>
        /// Gets command to cancel the copying.
        /// </summary>
        public DelegateCommand CancelCommand { get; }
        
        /// <summary>
        /// Gets command to resume the copying.
        /// </summary>
        public DelegateCommand ResumeCommand { get; }
         
        /// <summary>
        /// Gets or sets the minimum value of the progress bar.
        /// </summary>
        public int StartBar
        {
            get => this.startBar;
            set => this.SetProperty(ref this.startBar, value);
        }

        /// <summary>
        /// Gets or sets the current progress bar for copying a file.
        /// </summary>
        public int CurrentPercent
        {
            get => this.currentPercent;
            set => this.SetProperty(ref this.currentPercent, value);
        }

        /// <summary>
        /// Gets or sets the time left.
        /// </summary>
        public string TimeLeft
        {
            get => this.timeLeft;
            set => this.SetProperty(ref this.timeLeft, value);
        }

        /// <summary>
        /// Gets or sets the remainder.
        /// </summary>
        public string Remainder
        {
            get => this.remainder;
            set => this.SetProperty(ref this.remainder, value);
        }

        /// <summary>
        /// Gets or sets the average speed.
        /// </summary>
        public string AverageSpeed
        {
            get => this.averageSpeed;
            set => this.SetProperty(ref this.averageSpeed, value);
        }

        #endregion

        #region File Copy Controls

        /// <summary>
        /// Method to cancel copying.
        /// </summary>
        public void CopyCancel()
        {
            FileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Cancel);
        }

        /// <summary>
        /// Method to resume copying.
        /// </summary>
        public void CopyResume()
        {
            FileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Resume);
        }

        /// <summary>
        /// Method to suspend copying.
        /// </summary>
        public void CopySuspend()
        {
            FileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Pause);
        }

        #endregion

        #region Setup And Start

        /// <summary>
        /// Gets control on the copy thread.
        /// </summary>
        /// <param name="obj"> Expected two strings the source path and destination path. </param>
        private void SetupCopyFiles(object obj)
        {
            this.invoker.Execute(
                path =>
                {
                    this.SkippedFile = new ObservableCollection<CopyInfoModel>();
                    this.CopyReport = new ObservableCollection<CopyInfoModel>();

                    if (path is string[] address)
                    {
                        var source = new DirectoryInfo(address[0]);
                        var destination = new DirectoryInfo(address[1]);
                        FileCopier.StartCopyDeep(source.FullName, destination.FullName);
                        FileCopier.CopyProgressReport += this.FileCopier_CopyProgressReport;
                        FileCopier.CopyFileResult += this.FileCopier_CopyFileResult;
                    }
                },
            obj);
        }

        /// <summary>
        /// The event handle for initialization report a copy progress.
        /// </summary>
        /// <param name="sender"> The sender is <see cref="FileCopier"/> object. </param>
        /// <param name="progressInfo"> The copy progress information. </param>
        private void FileCopier_CopyFileResult(object sender, FileCopier.CopyInfo progressInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (progressInfo.Skipped)
                {
                    this.CopyReport.Add(new CopyInfoModel
                    {
                        Name = progressInfo.Name,
                        Source = progressInfo.Source,
                        Destination = progressInfo.Destination
                    });
                }
                else
                {
                    this.SkippedFile.Add(new CopyInfoModel
                    {
                        Name = progressInfo.Name,
                        Source = progressInfo.Source
                    });
                }
            });
        }

        /// <summary>
        /// The event handle for initialization report a copy progress.
        /// </summary>
        /// <param name="sender"> The sender is <see cref="FileCopier"/> object. </param>
        /// <param name="progressInfo"> The copy progress information. </param>
        private void FileCopier_CopyProgressReport(object sender, FileCopier.CopyInfo progressInfo)
        {
            this.CurrentPercent = (int)progressInfo.Percentage;
            this.AverageSpeed = Math.Round(ConverterBytes.AutoConvertBytes((decimal)progressInfo.AverageSpeed), 2) + " Mb/s";
            this.Remainder = $"Done {ConverterBytes.AutoConvertFormatBytes((decimal)progressInfo.ByteDone)} of {ConverterBytes.AutoConvertFormatBytes((decimal)progressInfo.FileSize)}";
            this.TimeLeft = progressInfo.TimeLeft.ToString(); //this.ConvertTimeElapsed(progressInfo.TimeLeft, "ru-RU");
        }

        /// <summary>
        /// The convert time elapsed.
        /// </summary>
        /// <param name="time"> The time. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="string"/>. </returns>
        private string ConvertTimeElapsed(TimeSpan time, string culture)
        {
            string format = "Time left: ";
            int milliseconds = time.Milliseconds;
            int seconds = time.Seconds;
            int minutes = time.Minutes;
            int hours = time.Hours;

            if (hours != 0)
            {
                format += hours + " h.";
            }

            if (minutes != 0)
            {
                format += hours != 0 ? ", " + minutes + " min." : minutes + " min.";
            }

            if (seconds != 0)
            {
                format += minutes != 0 ? ", " + seconds + " sec." : seconds + " sec.";
            }

            return format.Length > 10 ? format : "Calculating..";
        }

        #endregion
    }
}

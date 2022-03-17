
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
    using System.IO;
    using System.Windows;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Helper;
    using UnityCommander.Core.IO;
    using UnityCommander.Core.IO.Operations;

    /// <summary>
    /// The class is a view model for dialog window of the copy files.
    /// </summary>
    public class CopyProcessViewModel : BindableBase
    {
        #region Declaration Fields

        /// <summary>
        /// The manager.
        /// </summary>
        private readonly CopyManager copyManager = (CopyManager)Commander<CopyFiles>.GetManager();

        /// <summary>
        /// The invoker class instance.
        /// </summary>
        private readonly CopyFileInvoker invoker;

        /// <summary>
        /// Contains the current progress bar for copying a file.
        /// </summary>
        private int currentPercent;

        /// <summary>
        /// Contains the current progress bar for copying a file.
        /// </summary>
        private double exactPercent;

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
        /// The time left.
        /// </summary>
        private TimeSpan totalTimeLeft;

        /// <summary>
        /// The average speed.
        /// </summary>
        private string averageSpeed;

        /// <summary>
        /// The remainder.
        /// </summary>
        private string remainder;

        /// <summary>
        /// The remainder.
        /// </summary>
        private string current;

        private readonly MessageSendEvent messenger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProcessViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyProcessViewModel(IEventAggregator viewModelMessage)
        {
            this.invoker = new CopyFileInvoker();

            messenger = viewModelMessage.GetEvent<MessageSendEvent>();
            messenger.Subscribe(this.SetupCopyFiles);
            
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
        /// Gets or sets the integer percent for the file copy progress bar.
        /// </summary>
        public int CurrentPercent
        {
            get => this.currentPercent;
            set => this.SetProperty(ref this.currentPercent, value);
        }

        /// <summary>
        /// Gets or sets the exact percentage for the smoother file copy progress bar.
        /// </summary>
        public double ExactPercent
        {
            get => this.exactPercent;
            set => this.SetProperty(ref this.exactPercent, value);
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
        /// Gets or sets the time left.
        /// </summary>
        public TimeSpan TotalTimeLeft
        {
            get => this.totalTimeLeft;
            set => this.SetProperty(ref this.totalTimeLeft, value);
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
            this.copyManager.Cancel();
        }

        /// <summary>
        /// Method to resume copying.
        /// </summary>
        public void CopyResume()
        {
            this.copyManager.Resume();
        }

        /// <summary>
        /// Method to suspend copying.
        /// </summary>
        public void CopySuspend()
        {
            this.copyManager.Pause();
        }

        #endregion

        #region Setup And Start

        /// <summary>
        /// Gets control on the copy thread.
        /// </summary>
        /// <param name="path"> Expected two strings the source path and destination path. </param>
        private void SetupCopyFiles(object path)
        {
            //this.SkippedFile = new ObservableCollection<CopyInfoModel>();
            //this.CopyReport = new ObservableCollection<CopyInfoModel>();

            if (path is string[] address)
            {
                var source = new DirectoryInfo(address[0]);
                var destination = new DirectoryInfo(address[1]);

                this.copyManager.CopyFileReport += this.CopyFileReport;
                this.copyManager.Copy(source.FullName, destination.FullName);
                //CopyManager.CopyFileResult += this.CopyFileResult;
            }

            if (messenger.Contains(this.SetupCopyFiles))
            {
                messenger.Unsubscribe(this.SetupCopyFiles);
            }
        }

        /// <summary>
        /// The event handle for initialization report a copy progress.
        /// </summary>>
        /// <param name="progressInfo"> The copy progress information. </param>
        private void CopyFileResult(CopyInfo progressInfo)
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

                    this.current = progressInfo.Name;
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
        /// <param name="info"> The copy progress information. </param>
        private void CopyFileReport(CopyInfo info)
        {
            this.Remainder =
                $"{ConverterBytes.AutoConvertFormatBytes((decimal)info.TotalByteDone)} of {ConverterBytes.AutoConvertFormatBytes((decimal)info.TotalBytes)}";
            this.CurrentPercent = (int)Math.Round(info.TotalPercentage);
            this.ExactPercent = info.TotalPercentage;
            this.AverageSpeed = ConverterBytes.AutoConvertFormatBytes((decimal)info.AverageSpeed);
            this.TotalTimeLeft = info.TotalTimeLeft;
            //this.CurrentName = info.Name;
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

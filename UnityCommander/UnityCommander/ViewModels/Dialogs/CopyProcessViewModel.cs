
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
    using NLog;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using UnityCommander.Common.Commands;
    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Helper;
    using UnityCommander.Core.IO;
    using UnityCommander.Core.IO.Operations;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The class is a view model for dialog window of the copy files.
    /// </summary>
    public class CopyProcessViewModel : BindableBase, IDisposable
    {
        #region Declaration Fields

        /// <summary>
        /// The manager.
        /// </summary>
        private readonly CopyManager copyManager = (CopyManager)Commander<CopyFiles>.GetManager();

        private IGlobalCommandManager globalCommandManager;

        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

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

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Logger logger;

        private readonly MessageSendEvent messenger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProcessViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyProcessViewModel(
            IDialogService dialogService,
            IEventAggregator viewModelMessage,
            IGlobalCommandService globalCommandService,
            ModuleLogger moduleLogger)
        {
            this.logger = moduleLogger.GetLogger();
            this.dialogService = dialogService;
            this.globalCommandManager = globalCommandService.GetCommandManager();
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
        /// <param name="copyInfo"> Expected two strings the source path and destination path. </param>
        //private void SetupCopyFiles(object copyInfo)
        //{
        //    //this.SkippedFile = new ObservableCollection<CopyInfoModel>();
        //    //this.CopyReport = new ObservableCollection<CopyInfoModel>();

        //    //if (copyInfo is object[] address)
        //    //{
        //    //    var source = address[0] as DirectoryInfo;
        //    //    var destination = address[1] as DirectoryInfo;

        //    //    this.copyManager.CopyFileReport += this.CopyFileReport;
        //    //    this.copyManager.CopyFileFinish += this.CopyFileFinish;
        //    //    this.copyManager.CopySkipReplace += this.CopyFileSkipReplace;
        //    //    this.copyManager.Copy(source.FullName, destination.FullName);
        //    //    //CopyManager.CopyFileResult += this.CopyFileResult;
        //    //}
        //}

        /// <summary>
        /// Gets control on the copy thread.
        /// </summary>
        /// <param name="copyInfo"> Expected two strings the source path and destination path. </param>
        private void SetupCopyFiles(object obj)
        {
            if (obj is CopyInfo copyInfo)
            {
                if (copyInfo.DialogSkipReplaceStatus == CopyDialogSkipReplaceStatus.SkipAll)
                {
                    this.copyManager.Cancel();
                    return;
                }
                else if (copyInfo.DialogSkipReplaceStatus == CopyDialogSkipReplaceStatus.ReplaceAll)
                {
                    var dirName = copyInfo.FileInfo.Directory;

                    if (File.Exists(copyInfo.Destination))
                        File.Delete(copyInfo.Destination);
                    copyInfo.Destination = dirName.FullName;
                    this.copyManager.Resume();
                    return;
                }

                var source = copyInfo.Source;
                var destination = copyInfo.Destination;

                try
                {
                    //var fileCk = source.Replace(new FileInfo(source).Directory.FullName, destination);

                    //if (File.Exists(fileCk))
                    //    throw new Exception(($"Файл ({fileCk}) уже существует!"));

                    this.copyManager.CopyFileReport += this.CopyFileReport;
                    this.copyManager.CopyFileFinish += this.CopyFileFinish;
                    //this.copyManager.FileAlreadyExistsEvent += this.OnFileAlreadyExistsEvent;
                    this.copyManager.CopySkipReplace = OnFileAlreadyExistsEvent;
                    this.copyManager.Copy(source, destination);
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex.Message.ToString());
                }
            }
        }

        private void OnFileAlreadyExistsEvent(CopyInfo info)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.dialogService.ShowDialog("CopyDialogSkipReplace",
                    new OverrideDialogParameters(info), r => { });
            });
        }

        /// <summary>
        /// Обрабатывет событие которое возникает когда файл уже существует и требуется 
        /// дополнительные действия для подтверждения пропуска или замены существующего файла.
        /// </summary>>
        /// <param name="progressInfo"> Данный объект содержит подробную информацию об копируемом файле. </param>
        private void OnFileAlreadyExistsEvent(object sender, EventArgs e)
        {
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
        /// Обработчик события завершения копирования (файлов/папок).
        /// </summary>
        private void CopyFileFinish()
        {
            //var cmd = this.globalCommandManager.GetGlobalCommand("CloseCopyFileDialogCommand");
            //cmd.Command?.Execute(null);

            GlobalCommandExecute.OnGlobalCommandExecuteChanged(this, false);

            //var globalCmd = this.globalCommandManager.GetGlobalCommand(CommandNames.DirectoryUpdate);
            //globalCmd.Command.Execute(null);

            //if (globalCmd is GlobalCommand comm)
            //{
            //    comm.OnExecuteChanged(this);
            //}
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

        public void Dispose()
        {
            if (messenger.Contains(this.SetupCopyFiles))
            {
                messenger.Unsubscribe(this.SetupCopyFiles);
            }
        }

        #endregion
    }
}

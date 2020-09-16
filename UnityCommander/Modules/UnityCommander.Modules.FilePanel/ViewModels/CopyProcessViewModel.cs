
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using UnityCommander.Core;
    using UnityCommander.Core.IO;

    /// <summary>
    /// The class is a view model for dialog window of the copy files.
    /// </summary>
    public class CopyProcessViewModel : BindableBase
    {
        #region Declaration Fields

        /// <summary>
        /// Contains the current progress bar for copying a file.
        /// </summary>
        private int currentPercent;

        /// <summary>
        /// Contains the min value of the progress bar.
        /// </summary>
        private int startBar;

        /// <summary>
        /// The token source.
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// The token.
        /// </summary>
        private CancellationToken token;

        /// <summary>
        /// The report data about copying files.
        /// </summary>
        private ObservableCollection<CopyInfoModel> copyReport;

        /// <summary>
        /// The task status.
        /// </summary>
        private TaskStatus taskStatus;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProcessViewModel"/> class.
        /// This the signature of the constructor needed for communication with another a view models.
        /// </summary>
        /// <param name="viewModelMessage"> Communication parameter of the view models. </param>
        public CopyProcessViewModel(IEventAggregator viewModelMessage)
        {
            viewModelMessage.GetEvent<MessageSendEvent>().Subscribe(this.SetupFilesThread);
            FileDublicator.CopyingEvent += this.DirectoryItemsCopyingEvent;
            this.CopyReport = new ObservableCollection<CopyInfoModel>();
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

        #endregion

        #region File Copy Controls

        /// <summary>
        /// Method to cancel copying.
        /// </summary>
        public void CopyCancel()
        {
            FileDublicator.ChangeCopyStatus(Status.Cancel);
        }

        /// <summary>
        /// Method to resume copying.
        /// </summary>
        public void CopyResume()
        {
            FileDublicator.ChangeCopyStatus(Status.Resume);
        }

        /// <summary>
        /// Method to suspend copying.
        /// </summary>
        public void CopySuspend()
        {
            FileDublicator.ChangeCopyStatus(Status.Pause);
        }

        #endregion

        #region Setup And Start

        /// <summary>
        /// Gets control on the copy thread.
        /// </summary>
        /// <param name="obj"> Expected two strings the source path and destination path. </param>
        private async void SetupFilesThread(object obj)
        {
            if (obj is string[] address)
            {
                // Create the token to cancel the copy operation.
                this.tokenSource = new CancellationTokenSource();
                this.token = this.tokenSource.Token;

                // Initial data
                var source = new DirectoryInfo(address[0]);
                var destination = new DirectoryInfo(address[1]);
                var duplicator = new FileDublicator(source, destination);

                try
                {
                    // Create a task to copying
                    var task = Task<bool>.Factory.StartNew(() => duplicator.CreateDirectoryStructure(this.token), this.token);
                    this.taskStatus = task.Status;
                    bool cancelled = await task.ConfigureAwait(true);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The event handles report data on the copied file.
        /// </summary>
        /// <param name="sender"> Class <see cref="FileDublicator"/> is event initiator. </param>
        /// <param name="e"> Receives report data on the copied file. </param>
        private void DirectoryItemsCopyingEvent(object sender, CopyInfoEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.StartBar = 0;
                this.DisplayReport(e);
            });
        }

        /// <summary>
        /// Initials properties of the ProgressBar control 
        /// to display information on the copying file. 
        /// </summary>
        /// <param name="e"> Receives report data on the copied file. </param>
        private void DisplayReport(CopyInfoEventArgs e)
        {
            this.CopyReport.Add(e.ProgressBarInfo);
            this.StartBar = e.ProgressBarInfo.ProgressBar;
            this.CurrentPercent = e.ProgressBarInfo.ProgressBar;
        }

        #endregion
    }
}

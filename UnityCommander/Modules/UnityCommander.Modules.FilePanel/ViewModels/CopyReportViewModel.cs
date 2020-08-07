using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using UnityCommander.Core;
using UnityCommander.Core.IO;
using System.Windows;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class CopyReportViewModel : BindableBase
    {        
        /// <summary>
        /// Contains the report data for copied files.
        /// </summary>
        private ObservableCollection<CopyInfoModel> _copyReport;
        /// <summary>
        /// Gets or sets the report data for copied files.
        /// </summary>
        public ObservableCollection<CopyInfoModel> CopyReport 
        {
            get => this._copyReport;
            set => SetProperty(ref this._copyReport, value);
        }
        /// <summary>
        ///  Initializes a new instance of the <see cref="CopyReportViewModel"/> class.
        /// </summary>
        public CopyReportViewModel()
        {
            this.CopyReport = new ObservableCollection<CopyInfoModel>();
            FileDublicator.CopyingEvent += DirectoryItems_CopyingEvent;
        }

        #region Progress Bar
        /// <summary>
        /// Contains the current progress bar for copying a file.
        /// </summary>
        private int _currentPercent;
        /// <summary>
        /// Contains the min value of the progress bar.
        /// </summary>
        private int _startbar;
        /// <summary>
        /// Contains the max value of the progress bar.
        /// </summary>
        private int _endbar = 100;
        /// <summary>
        /// Gets or sets the minimum value of the progress bar.
        /// </summary>
        public int Startbar
        {
            get => this._startbar;
            set => SetProperty(ref this._startbar, value);
        }
        /// <summary>
        /// Gets or sets the maximum value of the progress bar.
        /// </summary>
        public int Endtbar
        {
            get => this._endbar;
            set => SetProperty(ref this._endbar, value);
        }
        /// <summary>
        /// Gets or sets the current progress bar for copying a file.
        /// </summary>
        public int CurrentPercent
        {
            get => this._currentPercent;
            set => SetProperty(ref this._currentPercent, value);
        }
        #endregion

        /// <summary>
        /// The event handles report data on the copied file.
        /// </summary>
        /// <param name="sender"> Class <see cref="FileDublicator"/> is event initiator. </param>
        /// <param name="e"> Receives report data on the copied file. </param>
        private void DirectoryItems_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Startbar = 0;
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
           // this.CopyReport.Add(e.CopyInfo);
            this.Startbar = e.ProgressBarInfo.ProgressBar;
            this.CurrentPercent = e.ProgressBarInfo.ProgressBar;
        }
    }
}

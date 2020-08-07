using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using UnityCommander.Core;
using UnityCommander.Core.IO;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using UnityCommander.Modules.FilePanel.Models;

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
            DirectoryItems.CopyingEvent += DirectoryItems_CopyingEvent;
        }

        #region Progress Bar

        private int _currentPerc;
        private int _startbar;
        private int _endbar = 100;

        public int Startbar
        {
            get => this._startbar;
            set => SetProperty(ref this._startbar, value);
        }

        public int Endtbar
        {
            get => this._endbar;
            set => SetProperty(ref this._endbar, value);
        }

        public int CurrentPerc
        {
            get => this._currentPerc;
            set => SetProperty(ref this._currentPerc, value);
        }

        #endregion CurrentProc

        /// <summary>
        /// The event handles report data on the copied file.
        /// </summary>
        /// <param name="sender"> Class <see cref="DirectoryItems"/> is event initiator. </param>
        /// <param name="e"> The report data. </param>
        private void DirectoryItems_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Startbar = 0;
                this.DisplayReport(e);
            });
        }

        private void DisplayReport(CopyInfoEventArgs e)
        {
            this.CopyReport.Add(e.CopyInfo);
            this.Startbar = e.CopyInfo.CopyStatus;
            this.CurrentPerc = e.CopyInfo.CopyStatus;
        }
    }
}

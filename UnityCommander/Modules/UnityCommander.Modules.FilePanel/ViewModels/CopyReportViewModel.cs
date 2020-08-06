using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using UnityCommander.Core;
using UnityCommander.Core.IO;
using System.ComponentModel;
using System.Windows.Threading;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class CopyReportViewModel : BindableBase
    {
        private ObservableCollection<CopyInfoModel> _copyReport;
        public ObservableCollection<CopyInfoModel> CopyReport 
        {
            get => this._copyReport;
            set => SetProperty(ref this._copyReport, value);
        }
        public string MyProperty { get; set; }
        /// <summary>
        ///  Initializes a new instance of the <see cref="CopyReportViewModel"/> class.
        /// </summary>
        public CopyReportViewModel()
        {
            this.CopyReport = new ObservableCollection<CopyInfoModel>();
            DirectoryItems.CopyingEvent += DirectoryItems_CopyingEvent;
        }
        /// <summary>
        /// The event handles report data on the copied file.
        /// </summary>
        /// <param name="sender"> Class <see cref="DirectoryItems"/> is event initiator. </param>
        /// <param name="e"> The report data. </param>
        private void DirectoryItems_CopyingEvent(object sender, CopyInfoEventArgs e)
        {
            Action action = () =>
            {
                this.CopyReport.Add(e.CopyInfo);
            };
            Dispatcher.CurrentDispatcher.Invoke(action);
        }
    }
}

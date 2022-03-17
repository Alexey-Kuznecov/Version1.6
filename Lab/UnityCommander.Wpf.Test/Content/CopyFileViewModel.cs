using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnityCommander.Test.Classes.IO;
using UnityCommander.Helper;
using UnityCommander.Helper.Mvvm.Base;
using UnityCommander.Helper.Converters;
using System.IO;

namespace UnityCommander.Wpf.Test.Content
{
    public class CopyFileViewModel : PropertiesChanged
    {
        private CopyManager copyManager;

        private int visiblePercent;
        
        private double doublePercent;

        private string currentSpeed;
        
        private string currentName;
        
        private string remainder;

        private TimeSpan currentTimeLeft;

        public CopyFileViewModel()
        {
            this.copyManager = new CopyManager();
            this.Source = "D:\\Works\\WPF\\CopyTest\\Source";
            this.Destination = "D:\\Works\\WPF\\CopyTest\\Target";
        }

        public string Remainder
        {
            get => this.remainder;
            set
            {
                this.remainder = value;
                this.OnPropertyChanged("Remainder");
            }
        }
        public int VisiblePercent
        {
            get => this.visiblePercent;
            set
            {
                this.visiblePercent = value;
                this.OnPropertyChanged("VisiblePercent");
            }
        }

        public double DoublePercent
        {
            get => this.doublePercent;
            set
            {
                this.doublePercent = value;
                this.OnPropertyChanged("DoublePercent");
            }
        }

        public string CurrentSpeed
        {
            get => this.currentSpeed;
            set
            {
                this.currentSpeed = value;
                this.OnPropertyChanged("CurrentSpeed");
            }
        }
        public string CurrentName
        {
            get => this.currentName;
            set
            {
                this.currentName = value;
                this.OnPropertyChanged("CurrentName");
            }
        }
        public TimeSpan CurrentTimeLeft
        {
            get => this.currentTimeLeft;
            set
            {
                this.currentTimeLeft = value;
                this.OnPropertyChanged("CurrentTimeLeft");
            }
        }
        public string Source { get; set; }
        public string Destination { get; set; }

        public ICommand StartCommand => new RelayCommand(obj =>
        {
            this.StartCopy();
        });

        public ICommand PauseCommand => new RelayCommand(obj =>
        {
            this.copyManager.Pause();
        });

        public ICommand ResumeCommand => new RelayCommand(obj =>
        {
            this.copyManager.Resume();
        });

        public ICommand CancelCommand => new RelayCommand(obj =>
        {
            this.copyManager.Cancel();
        });

        public ICommand RestartCommand => new RelayCommand(obj =>
        {
            Directory.Delete(this.Destination, true);
            this.StartCopy();
        });

        private void StartCopy()
        {
            if (!Directory.Exists(Destination))
            {
                Directory.CreateDirectory(this.Destination);
            }

            this.copyManager.CopyFileReport += this.CopyFileResult;
            this.copyManager.Copy(this.Source, this.Destination);
        }

        private void CopyFileResult(CopyInfo info)
        {
            this.Remainder =
                $"{ConverterBytes.AutoConvertFormatBytes((decimal)info.TotalByteDone)} of {ConverterBytes.AutoConvertFormatBytes((decimal)info.TotalBytes)}";
            this.VisiblePercent = (int)Math.Round(info.TotalPercentage);
            this.DoublePercent = info.TotalPercentage;
            this.CurrentSpeed = ConverterBytes.AutoConvertFormatBytes((decimal)info.AverageSpeed);
            this.CurrentTimeLeft = info.TotalTimeLeft;
            this.CurrentName = info.Name;
        }
    }
}

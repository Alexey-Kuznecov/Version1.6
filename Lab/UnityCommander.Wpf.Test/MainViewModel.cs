using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UnityCommander.Helper.Mvvm.Base;

namespace UnityCommander.Wpf.Test
{
    using Content;

    public class MainViewModel : PropertiesChanged
    {
        private UserControl selectedCurrentTest;

        private UserControl testContent;

        public MainViewModel()
        {
            var copyFile = new CopyFileControl();
            this.CollectionTests = new List<UserControl>
            {
                copyFile,
                new DragDropControl()
            };

            this.TestContent = copyFile;
        }

        public List<UserControl> CollectionTests { get; set; }

        public UserControl SelectedCurrentTest
        {
            get => this.selectedCurrentTest;
            set
            {
                this.selectedCurrentTest = value;
                this.TestContent = this.selectedCurrentTest;
                this.OnPropertyChanged("SelectedCurrentTest");
            }
        }

        public UserControl TestContent
        {
            get => this.testContent;
            set 
            {
                this.testContent = value;
                this.OnPropertyChanged("TestContent"); 
            }
        }
    }
}

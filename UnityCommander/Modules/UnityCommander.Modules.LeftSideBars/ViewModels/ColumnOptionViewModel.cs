using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public class ColumnOptionViewModel : BindableBase
    {
        private bool isChecked;

        public ColumnOptionViewModel()
        {
        }

        public bool IsChecked 
        { 
            get 
            {
                return this.isChecked;
            } 
            set 
            {
                this.SetProperty(ref this.isChecked, value);
                
                if (value)
                    MessageBox.Show("IsCk");
                else
                    MessageBox.Show("IsNotCk");
            } 
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using UnityCommander.Mvvm.Base;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class DropListPopupViewMdoel : PropertiesChanged
    {
        private List<DropListPopupModel> controlList;
        private DropListPopupModel selectItem;
        private bool isPopupOpen;
        private bool popupIsFocus;

        public DropListPopupViewMdoel(List<DropListPopupModel> dropListPopups)
        {
            this.IsPopupOpen = true;
            this.ControlList = dropListPopups;
        }

        public List<DropListPopupModel> ControlList 
        { 
            get => this.controlList;
            set 
            {
                this.controlList = value;
                this.OnPropertyChanged("ControlList");
            }
        }

        public DropListPopupModel SelectItem
        {
            get => this.selectItem;
            set
            {
                this.IsPopupOpen = false;
                this.selectItem = value;
                this.OnPropertyChanged("SelectItem");
            }
        }

        public bool IsPopupOpen
        {
            get => this.isPopupOpen;
            set
            {
                this.isPopupOpen = value;
                this.OnPropertyChanged("IsPopupOpen");
            }
        }
    }
}

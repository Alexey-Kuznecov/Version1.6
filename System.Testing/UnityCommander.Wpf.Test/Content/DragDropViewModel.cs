using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AlexeyKuznecov.Library.Mvvm.Base;
using Components.DragDrop.Full;

namespace UnityCommander.Wpf.Test.Content
{
    public class DirectoryModel
    {
        public string Name { get; set; }
    }

    public class DragDropViewModel : PropertiesChanged, IDropTarget
    {
        private GridView directoryListContainer;
        private GridView directoryListContainerRight;

        private ObservableCollection<DirectoryModel> directoryList;
        private ObservableCollection<DirectoryModel> directoryListRight;

        public DragDropViewModel()
        {
            this.DirectoryList = new ObservableCollection<DirectoryModel>()
            {
                new DirectoryModel { Name = "Directory Name 1" },
                new DirectoryModel { Name = "Directory Name 2" },
                new DirectoryModel { Name = "Directory Name 3" },
                new DirectoryModel { Name = "Directory Name 4" },
                new DirectoryModel { Name = "Directory Name 5" },
            };

            this.DirectoryListRight = new ObservableCollection<DirectoryModel>()
            {
                new DirectoryModel { Name = "Directory Name 10" },
                new DirectoryModel { Name = "Directory Name 11" },
                new DirectoryModel { Name = "Directory Name 12" },
                new DirectoryModel { Name = "Directory Name 13" },
                new DirectoryModel { Name = "Directory Name 14" },
            };

            this.DirectoryListContainer = new GridView();
            this.DirectoryListContainer.Columns.Add(new GridViewColumn()
            {
                Header = "Name",
                Width = 200,
                CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnExtensionDataTemplate")
            });

            this.DirectoryListContainerRight = new GridView();
            this.DirectoryListContainerRight.Columns.Add(new GridViewColumn()
            {
                Header = "Name",
                Width = 200,
                CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnExtensionDataTemplate")
            });
        }

        public GridView DirectoryListContainer
        {
            get { return directoryListContainer; }
            set 
            {
                this.directoryListContainer = value;
                this.OnPropertyChanged("DirectoryListContainer");
            }
        }

        public GridView DirectoryListContainerRight
        {
            get { return directoryListContainerRight; }
            set
            {
                this.directoryListContainerRight = value;
                this.OnPropertyChanged("DirectoryListContainerRight");
            }
        }

        public ObservableCollection<DirectoryModel> DirectoryList
        {
            get { return directoryList; }
            set
            {
                this.directoryList = value;
                this.OnPropertyChanged("DirectoryList");
            }
        }

        public ObservableCollection<DirectoryModel> DirectoryListRight
        {
            get { return directoryListRight; }
            set
            {
                this.directoryListRight = value;
                this.OnPropertyChanged("DirectoryListRight");
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var adorner = AdornerLayer.GetAdornerLayer(dropInfo.VisualTarget);

            if (adorner == null)
            {
                this.CreateAdornerLayer(dropInfo.VisualTarget);
            }

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Copy;
        }

        public void Drop(IDropInfo dropInfo)
        {
        }

        /// <summary>
        /// The create adorner layer.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        private void CreateAdornerLayer(UIElement element)
        {
            var listBox = element as ListView;
            var ad = new AdornerDecorator();

            if (listBox?.Parent is Grid parent)
            {
                parent.Children.Remove(listBox);
                ad.Child = listBox;
                parent.Children.Add(ad);
            }
        }
    }
}

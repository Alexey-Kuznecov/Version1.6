using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AlexeyKuznecov.Library.Mvvm.Base;
using Components.Tab;
using TabControl = Components.Tab.TabControl;

namespace UnityCommander.Wpf.Test.Content
{
    public class TabPanelViewModel : PropertiesChanged
    {
        private TabCollection tabCollection;

        public TabPanelViewModel()
        {
            var collection = new TabCollection();

            collection.Add(this.CreateTabControl("Tab 1"));
            collection.Add(this.CreateTabControl("Tab 2"));
            collection.Add(this.CreateTabControl("Tab 3"));
            collection.Add(this.CreateTabControl("Tab 4"));
            collection.Add(this.CreateAddTabControl());

            this.TabCollection = collection;
        }

        public ICommand ActivateTabCommand =>
           new RelayCommand(
               obj =>
               {
                  
               });

        public ICommand CloseTabCommand =>
          new RelayCommand(
               obj =>
               {
                   this.TabCollection.Remove((TabControl)obj);
               });

        public ICommand AddNewTabCommand =>
            new RelayCommand(
               panel =>
               {
                   if (panel is TabPanel tabPanel)
                   {
                       tabPanel.Collection.Add(CreateTabControl("New Tab"));
                   }
               });

        public TabCollection TabCollection
        {
            get => this.tabCollection;
            set
            {
                this.tabCollection = value;
                this.OnPropertyChanged("TabCollection");
            }
        }

        private TabControl CreateTabControl(string content)
        {
            TabControl button = new TabControl
            {
                Content = content,
                Margin = new Thickness(0, 0, 1, 0),
                Background = new SolidColorBrush(Color.FromRgb(224, 229, 241)),
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                Tag = content,
                Command = this.ActivateTabCommand,
                CloseCommand = this.CloseTabCommand
            };
            
            button.TabClick += this.OnTabControlClick;
            return button;
        }

        private AddTabControl CreateAddTabControl()
        {
            var button = new AddTabControl
            {
                Content = "+",
                Margin = new Thickness(0, 0, 1, 0),
                Background = null,
                Foreground = new SolidColorBrush(Color.FromRgb(47, 78, 79)),
                BorderBrush = null,
                Command = this.AddNewTabCommand
            };

            return button;
        }

        private void OnTabControlClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}

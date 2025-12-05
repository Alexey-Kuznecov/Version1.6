
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class DefaultContainer
    {
        private ObservableCollection<BaseContainer> containers;

        public void GetColumn(Action<ObservableCollection<BaseContainer>, Exception> callback)
        {
            callback(this.containers, null);
        }

        public void AddColumn(BaseContainer column)
        {
            this.containers.Add(column);
        }

        protected virtual void InitialData()
        {
            FrameworkElementFactory totalAmauntTemplate = new (typeof(TextBlock));
            FrameworkElementFactory letterTemplate      = new (typeof(TextBlock));
            FrameworkElementFactory usedSpaceTemplate   = new (typeof(TextBlock));
            FrameworkElementFactory freeSpaceTemplate   = new (typeof(TextBlock));

            letterTemplate     .SetBinding(TextBlock.TextProperty, new Binding { Path = new ("Letter") });   
            usedSpaceTemplate  .SetBinding(TextBlock.TextProperty, new Binding { Path = new ("UsedSpace") });         
            freeSpaceTemplate  .SetBinding(TextBlock.TextProperty, new Binding { Path = new ("FreeSpace") });
            totalAmauntTemplate.SetBinding(TextBlock.TextProperty, new Binding { Path = new ("TotalAmount") }); 

            this.containers = new ObservableCollection<BaseContainer>
            {
                new ()
                {
                    Content = "Icon",
                    Template = new GridViewColumn
                    {
                        Header = "Icon",
                        Width = 100,
                        CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnIconDataTemplate")
                    }
                },
                new ()
                {
                    Content = "Letter",
                    Template = new GridViewColumn
                    {
                        Header = "Letter",
                        Width = 100,
                        CellTemplate = new ()
                        {
                            VisualTree = letterTemplate
                        }
                    }
                },
                new ()
                {
                    Content = "Free Space",
                    Template = new GridViewColumn
                    {
                        Header = "Free Space",
                        Width = 100,
                        CellTemplate = new ()
                        {
                            VisualTree = freeSpaceTemplate
                        }
                    }
                },
                new ()
                {
                    Content = "Used Space",
                    Template = new GridViewColumn
                    {
                        Header = "Used Space",
                        Width = 100,
                        CellTemplate = new ()
                        {
                            VisualTree = usedSpaceTemplate
                        }
                    }
                },
                new ()
                {
                    Content = "Total Space",
                    Template = new GridViewColumn
                    {
                        Header = "Total Space",
                        Width = 100,
                        CellTemplate = new ()
                        {
                            VisualTree = totalAmauntTemplate
                        }
                    }
                }
            };
        }
    }
}

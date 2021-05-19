using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UnityCommander.Common.Models
{
    public class DirectoryTemplate
    {
        public DataTemplate ColumnTemplate { get; private set; }

        public void SetColumnTemplate(string templName)
        {
            ColumnTemplate = (DataTemplate)Application.Current.FindResource(templName);
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace AIconBrowser.Components.InputBox
{
    public class ActionNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ActionsDictionary ad = new ActionsDictionary();
            if (value != null)
                return ad.GetValue((Actions)value);
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ActionsDictionary ad = new ActionsDictionary();
            if (value != null)
                return ad.GetKey((string)value);
            return null;
        }
    }
}

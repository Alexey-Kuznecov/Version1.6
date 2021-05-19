using System;
using System.Collections.Generic;
using System.Text;

namespace AkuzIcons
{
    using System.Windows;
    using System.Windows.Media;

    public class IconProvider
    {
        public DrawingBrush GetIcon()
        {
            //var resource = new ResourceDictionary
            //{
            //    Source = new Uri("/AkuzIcons;component/Shell/Icons.xaml", UriKind.RelativeOrAbsolute)
            //};
            var resource = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Icons.xaml") };
            return null;
        }
    }
}

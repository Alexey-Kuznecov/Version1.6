using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityCommander.Common;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class ColumnValueConverter : BaseConverter<ColumnValueConverter>
    {
        private static int outputOrder;
     
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (value as ICollection<object>).ToList();

            var values = default(object);

            if (outputOrder > collection.Count - 1)
                outputOrder = 0;

            if (collection.Count > 0)
                values = collection[outputOrder++];

            return values;
        }
    }
}

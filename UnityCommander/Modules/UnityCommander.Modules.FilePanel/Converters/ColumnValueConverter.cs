
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityCommander.WPF;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class ColumnValueConverter : BaseConverter<ColumnValueConverter>
    {
        // Stateful workaround for WPF converter invocation order.
        // Reset when panel layout changes so the next visual pass starts from zero.
        public static int outputOrder;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ICollection<object> collection || collection.Count == 0)
                return null;

            var list = collection.ToList();

            if (outputOrder >= list.Count)
                outputOrder = 0;

            var index = outputOrder;

            // TODO: Временная компенсация циклического сдвига.
            // Нужно выяснить, почему первый вызов Convert начинается
            // не с первого элемента коллекции.
            index = (index + list.Count - 1) % list.Count;

            outputOrder++;

            return list[index];
        }
    }
}

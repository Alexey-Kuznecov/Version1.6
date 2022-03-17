using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NSwag.Collections;

namespace UnityCommander.Common.Styling.Converters
{
    public class ColumnValueConverter : BaseConverter<ColumnValueConverter>
    {
        private static int outputOrder;
       
        //private static ObservableDictionary<string, object> collection;

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (value as ICollection<object>).ToList();

            var values = default(object);

            if (outputOrder > collection.Count - 1)
                outputOrder = 0;
                
            values = collection[outputOrder++];
            return values;
        }

        private string GetColumnValue(int number)
        {
            //int counter = 0;

            //foreach (var item in collection.Keys)
            //{
            //    if (counter == number)
            //    {
            //        if (counter == collection.Count - 1)
            //        {
            //            outputOrder = -1;
            //        }

            //        return item;
            //    }

            //    counter++;
            //}

            return null;
        }
    }
}

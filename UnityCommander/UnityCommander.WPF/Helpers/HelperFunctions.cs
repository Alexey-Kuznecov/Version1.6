
namespace UnityCommander.WPF.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using NSwag.Collections;


    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public static class HelperFunctions
    {
        [DebuggerStepThrough]
        public static string ClearExtension(string path)
        {
            var result = path;
            do
            {
                path = result;
                result = Path.Combine(Path.GetDirectoryName(path) ?? throw new InvalidOperationException(), Path.GetFileNameWithoutExtension(path));
            }
            while (result != path);
            return result;
        }

        [DebuggerStepThrough]  
        public static SolidColorBrush StringFormatToSolidColor(this string value)
        {
            BrushConverter converter = new BrushConverter();          
            SolidColorBrush solid = (SolidColorBrush)converter.ConvertFromString(value);
            return solid;
        }

        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void RemoveFromParent(this FrameworkElement item)
        {
            var parentItemsControl = (WrapPanel) item?.Parent;
            parentItemsControl?.Children.Remove(item as UIElement);
        }

        [DebuggerStepThrough]
        public static ResourceDictionary GetResourceDictionary(string resourceName)
        {
            Collection<ResourceDictionary> collMergedDictionaries = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary resourceDictionary = collMergedDictionaries.Single(p => p.Source.ToString().Contains(resourceName));
            return resourceDictionary;
        }

        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void MessageBoxExtension(object obj)
        {
            MessageBox.Show(obj.GetHashCode().ToString(), obj.GetType().FullName);
        }

        [DebuggerStepThrough]
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collect)
        {
            var ob = new ObservableCollection<T>();
            foreach (var item in collect)
            {
                ob.Add(item);
            }

            return ob;
        }

        [DebuggerStepThrough]
        public static List<T> MergeList<T>(this List<T> container, List<T> combined)
        {
            foreach (var item in combined)
            {
                container.Add(item);
            }

            return container;
        }

        [DebuggerStepThrough]
        public static int MaxValue(this IEnumerable collection)
        {
            int max = 0;
            foreach (var o in collection)
            {    
                if ((int)o > max)
                {
                    max = (int)o;
                }
            }

            return max;
        }

        [DebuggerStepThrough]
        public static string Concat(this string str, object strB)
        {
            return str + strB;
        }

        public static object ExtractEach(this IEnumerable collection)
        {
            foreach (var item in collection)
            {
                return item;
            }

            return null;
        }

        public static Dictionary<TKey, TValue> ConvertToDictionary<TKey,TValue>(this IDictionary<TKey, TValue> oldPairs)
        {
            Dictionary<TKey, TValue> newPairs = new ();

            foreach (var item in oldPairs)
            {
                newPairs.Add(item.Key, item.Value);
            }

            return newPairs;
        }

        public static ObservableDictionary<TKey, TValue> ConvertToObservableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> oldPairs)
        {
            ObservableDictionary<TKey, TValue> newPairs = new();

            foreach (var item in oldPairs)
            {
                newPairs.Add(item.Key, item.Value);
            }

            return newPairs;
        }
    }
}

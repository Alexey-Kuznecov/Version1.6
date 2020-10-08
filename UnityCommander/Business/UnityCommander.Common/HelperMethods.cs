
namespace UnityCommander.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The helper methods.
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Parses the path into nested paths that its make up.
        /// </summary>
        /// <param name="dirPath">  Expected the path to the directory. </param>
        /// <returns> An array of nested paths that make up the path. </returns>
        public static string[] ParsePath(string dirPath)
        {
            string[] splitPath = dirPath.Split('\\');
            string[] paths = new string[!splitPath.Contains(string.Empty) ? splitPath.Length : splitPath.Length - 1];
            string newPath = string.Empty;
            int i = 0;

            while (i < splitPath.Length)
            {
                if (splitPath[i] == string.Empty) break;

                newPath = newPath.Equals(string.Empty)
                              ? splitPath[i] + "\\"
                              : newPath.Replace(paths[i - 1], newPath + splitPath[i] + "\\");

                paths[i++] = newPath;
            }

            return paths;
        }

        /// <summary>
        /// Deletes the <c>path</c> and file extensions, leaves only the name.
        /// </summary>
        /// <param name="path"> The <c>path</c> or file name. </param>
        /// <returns> Returns the file name. </returns>
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

        /// <summary>
        /// Converts a hexadecimal <c>value</c> to a brush color.
        /// </summary>
        /// <param name="value"> Hexadecimal <c>value</c>. </param>
        /// <returns> Returns the color of the brush. </returns>  
        [DebuggerStepThrough]
        public static SolidColorBrush StringFormatToSolidColor(this string value)
        {
            BrushConverter converter = new BrushConverter();
            SolidColorBrush solid = (SolidColorBrush)converter.ConvertFromString(value);
            return solid;
        }

        /// <summary>
        /// Solves the problem: The specified element is already a logical child
        /// of another element. Disconnect it first.
        /// </summary>
        /// <param name="item"> Any descendant of the Controls class, for example (Button). </param>
        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void RemoveFromParent(this FrameworkElement item)
        {
            var parentItemsControl = (WrapPanel)item?.Parent;
            parentItemsControl?.Children.Remove(item as UIElement);
        }

        /// <summary>
        /// Searches the dictionary for links from the combined resource dictionaries,
        /// this method does not look for resources in the main dictionary of <c>App.xaml</c>.
        /// </summary>
        /// <param name="resourceName"> The name of the resource dictionary. </param>
        /// <returns> Returns a resource dictionary. </returns>
        [DebuggerStepThrough]
        public static ResourceDictionary GetResourceDictionary(string resourceName)
        {
            Collection<ResourceDictionary> collMergedDictionaries = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary resourceDictionary = collMergedDictionaries.Single(p => p.Source.ToString().Contains(resourceName));
            return resourceDictionary;
        }

        /// <summary>
        /// Displays the hash code and type.
        /// </summary>
        /// <param name="obj">
        /// Any <see langword="object"/>.
        /// </param>
        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void MessageBoxExtension(object obj)
        {
            MessageBox.Show(obj.GetHashCode().ToString(), obj.GetType().FullName);
        }

        /// <summary>
        /// Packs enumerator items in the observable collection. Used like the
        /// standard ToList extension method.
        /// </summary>
        /// <typeparam name="T"> Any type. </typeparam>
        /// <param name="collect"> Enumerated collection. </param>
        /// <returns> Gets the tracked collection. </returns>
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

        /// <summary>
        /// Joins two collection to one.
        /// </summary>
        /// <typeparam name="T">The <see langword="object"/> type of collection.</typeparam>
        /// <param name="container">The collection as container to merge.</param>
        /// <param name="combined">The collection to be merged into a container..</param>
        /// <returns> Returns the combined collection. </returns>
        [DebuggerStepThrough]
        public static List<T> MergeList<T>(this List<T> container, List<T> combined)
        {
            foreach (var item in combined)
            {
                container.Add(item);
            }

            return container;
        }

        /// <summary>
        /// Find the maximum number in an array or collections consisting of integers.
        /// </summary>
        /// <param name="collection">Collection IEnumerable <see langword="interface"/>.</param>
        /// <returns>Return max number.</returns>
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

        /// <summary>
        /// The method concatenates two strings.
        /// </summary>
        /// <param name="str"> First string. </param>
        /// <param name="strB"> Second string. </param>
        /// <returns> Concatenated strings. </returns>
        [DebuggerStepThrough]
        public static string Concat(this string str, object strB)
        {
            return str + strB;
        }

        /// <summary>
        /// The select each.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ExtractEach(this IEnumerable collection)
        {
            foreach (var item in collection)
            {
                return item;
            }

            return null;
        }
    }
}

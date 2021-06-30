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

namespace AIconBrowser.Help
{
    public static class HelperFunctions
    {
        /// <summary>
        /// Удаляет путь и расширения файла, оставляет только имя.
        /// </summary>
        /// <param name="path">Путь или имя файла.</param>
        /// <returns>Возвращает имя файла.</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), DebuggerStepThrough]
        public static string ClearExtension(string path)
        {
            var result = path;
            do
            {
                path = result;
                result = Path.Combine(
                    Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path));
            }
            while (result != path);
            return result;
        }
        
        /// <summary>
        /// Преобразует шестнадцатеричное значение в цвет кисти.
        /// </summary>
        /// <param name="value">Шестнадцатеричное значение.</param>
        /// <returns>Возращает цвет кисти.</returns>  
        [DebuggerStepThrough]  
        public static SolidColorBrush StringFormatToSolidColor(this string value)
        {
            BrushConverter converter = new BrushConverter();          
            SolidColorBrush solid = (SolidColorBrush)converter.ConvertFromString(value);
            return solid;
        }
        
        /// <summary>
        /// Решает проблему: Указанный элемент уже является логическим дочерним для другого элемента. Сначала отсоедините его.
        /// </summary>
        /// <param name="item">Любой потомок класса Controls например (Кнопка).</param>
        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void RemoveFromParent(this FrameworkElement item)
        {
            var parentItemsControl = (WrapPanel) item?.Parent;
            parentItemsControl?.Children.Remove(item as UIElement);
        }
        
        /// <summary>
        /// Ищет словарь по ссылкам объяденненых словарей ресурсов,
        /// данным метод не ищет ресурсы в главном словаре App.xaml.
        /// </summary>
        /// <param name="resourceName">Имя словаря ресурсов.</param>
        /// <returns>Возвращает словарь ресурсов.</returns>
        [DebuggerStepThrough]
        public static ResourceDictionary GetResourceDictionary(string resourceName)
        {
            Collection<ResourceDictionary> collMergedDictionaries = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary resourceDictionary = collMergedDictionaries.Single(p => p.Source.ToString().Contains(resourceName));
            return resourceDictionary;
        }
        
        /// <summary>
        /// Выводит хеш-код и тип.
        /// </summary>
        [Conditional("DEBUG"), DebuggerStepThrough]
        public static void MessageBoxExtension(object obj)
        {
            MessageBox.Show(obj.GetHashCode().ToString(), obj.GetType().FullName);
        }
        
        /// <summary>
        /// Упаковывает элементы перечислителя в отслеживаемую коллекцию. 
        /// Используется как и стандартный метод расширения ToList.
        /// </summary>
        /// <typeparam name="T">Любой тип.</typeparam>
        /// <param name="collect">Перечисляемая коллекция.</param>
        /// <returns>Возвращает отслеживаемую коллекцию коллекция</returns>
        [DebuggerStepThrough]
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collect )
        {
            var ob = new ObservableCollection<T>();
            foreach (var item in collect)
                ob.Add(item);
            return ob;
        }
        
        /// <summary>
        /// Joins two collection to one.
        /// </summary>
        /// <typeparam name="T">The object type of collection.</typeparam>
        /// <param name="container">The collection as container to merge.</param>
        /// <param name="combined">The collection to be merged into a container..</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static List<T> MergeList<T>(this List<T> container, List<T> combined)
        {
            foreach (var item in combined)
                container.Add(item);
            return container;
        }
        
        /// <summary>
        /// Find the maximum number in an array or collections consisting of integers.
        /// </summary>
        /// <param name="collection">Collection IEnumerable interface.</param>
        /// <returns>Return max number.</returns>
        [DebuggerStepThrough]
        public static int MaxValue(this IEnumerable collection)
        {
            int max = 0;
            foreach (var o in collection)
                if ((int)o > max)
                    max = (int)o;
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
            return str + strB.ToString();
        }
    }
}

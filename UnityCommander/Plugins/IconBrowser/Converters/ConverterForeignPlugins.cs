
// ReSharper disable All
namespace IconBrowser.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    //[DebuggerStepThrough]
    class ConverterForeignPlugins
    {
        /// <summary>
        /// Метод конвертирует xaml разметку сгенерированную плагином 
        /// для илюстратора XamlExport64, в читаемый вид для редактора иконок.
        /// </summary>
        /// <param name="rootXaml">Корневой элементы плагина XamlExport64.</param>
        /// <returns>Конкатенацию строк сожержащих путь.</returns>
        public static string XamlExport64Path(Viewbox rootXaml)
        {
            Canvas canvas = rootXaml.Child as Canvas;
            string paths = " ";
            object check = null;

            while (check?.GetType() != typeof(Path))
            {
                if (canvas != null)
                    foreach (var child in canvas.Children)
                    {
                        check = child;
                        if (child is Path)
                            paths += (child as Path).Data.ToString().Substring(2).Replace(',', '.').Replace(';', ',');
                        else
                            canvas = (Canvas) child;
                    }
            }
            return paths;
        }
        /// <summary>
        /// Метод конвертирует xaml разметку сгенерированную плагином 
        /// для илюстратора XamlExport64, в читаемый вид для редактора иконок.
        /// </summary>
        /// <param name="rootXaml">Корневой элементы плагина XamlExport64.</param>
        /// <returns>Конкатенацию строк сожержащих путь.</returns>
        public static List<Path> XamlExport64PathArray(Viewbox rootXaml)
        {
            Canvas canvas = rootXaml.Child as Canvas;
            List<Path> paths = new List<Path>();
            object check = null;

            while (check?.GetType() != typeof(Path))
            {
                if (canvas != null)
                    foreach (var child in canvas.Children)
                    {
                        check = child;
                        if (child is Path)
                            paths.Add(new Path { Fill = (child as Path).Fill, Data = (child as Path).Data });
                        else
                        {
                            var check2 = child as Canvas;
                            if (check2 != null)
                                canvas = (Canvas)child;
                            else
                               throw new InvalidCastException("Неудалось прочитать файл, недопустимая разметка: " + child);
                        }
                    }
            }
            return paths;
        }
    }
}

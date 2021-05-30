using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControlLibrary
{
    /// <summary>
    /// Выполните шаги 1a или 1b, а затем 2, чтобы использовать этот пользовательский элемент управления в файле XAML.
    ///
    /// Шаг 1a. Использование пользовательского элемента управления в файле XAML, существующем в текущем проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary"
    ///
    ///
    /// Шаг 1б. Использование пользовательского элемента управления в файле XAML, существующем в другом проекте.
    /// Добавьте атрибут XmlNamespace в корневой элемент файла разметки, где он 
    /// будет использоваться:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary;assembly=CustomControlLibrary"
    ///
    /// Потребуется также добавить ссылку из проекта, в котором находится файл XAML,
    /// на данный проект и заново выполнить построение во избежание ошибок компиляции:
    ///
    ///     Щелкните правой кнопкой мыши нужный проект в обозревателе решений и выберите
    ///     "Добавить ссылку"->"Проекты"->[Выберите этот проект]
    ///
    ///
    /// Шаг 2)
    /// Продолжайте дальше и используйте элемент управления в файле XAML.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>

    /// <summary>
    /// Grid splitter that show or hides the following row when the visibility of the splitter is changed. 
    /// </summary>
    public class HidableGridSplitter : GridSplitter
    {
        GridLength height;

        static HidableGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HidableGridSplitter), new FrameworkPropertyMetadata(typeof(HidableGridSplitter)));
        }

        public HidableGridSplitter()
        {
            this.IsVisibleChanged += HideableGridSplitter_IsVisibleChanged;
            this.Initialized += HideableGridSplitter_Initialized;
        }

        void HideableGridSplitter_Initialized(object sender, EventArgs e)
        {
            ////Cache the initial RowDefinition height,
            ////so it is not always assumed to be "Auto"
            //Grid parent = base.Parent as Grid;
            //if (parent == null) return;
            //int rowIndex = Grid.GetRow(this);
            //if (rowIndex + 1 >= parent.RowDefinitions.Count) return;
            //var lastRow = parent.RowDefinitions[rowIndex + 1];
            //height = lastRow.Height;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            // draw your stuff here
        }

        void HideableGridSplitter_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Grid parent = base.Parent as Grid;
            //if (parent == null) return;

            //int rowIndex = Grid.GetRow(this);

            //if (rowIndex + 1 >= parent.RowDefinitions.Count) return;

            //var lastRow = parent.RowDefinitions[rowIndex + 1];

            //if (this.Visibility == Visibility.Visible)
            //{
            //    lastRow.Height = height;
            //}
            //else
            //{
            //    height = lastRow.Height;
            //    lastRow.Height = new GridLength(0);
            //}
        }
    }
}

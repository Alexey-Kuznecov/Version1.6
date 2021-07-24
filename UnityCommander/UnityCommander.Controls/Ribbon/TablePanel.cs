
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// TODO The table.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class Table : Panel
    {
        /// <summary>
        /// The rows.
        /// </summary>
        private double[] rows;

        /// <summary>
        /// The columns.
        /// </summary>
        private double[] columns;

        /// <summary>
        /// The real columns.
        /// </summary>
        private int realColumns;

        /// <summary>
        /// The real rows.
        /// </summary>
        private int realRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        public Table()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the columns number.
        /// </summary>
        public int ColumnsNumber { get; set; }

        /// <summary>
        /// Gets or sets the rows number.
        /// </summary>
        public int RowsNumber { get; set; }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            CalculateColumns();
            
            // некий ограничитель
            int constraint = 300;
            
            // распределяем пространство панели равномерно
            Size childConstraint = new Size(constraint / realColumns, constraint / realRows);

            int rowcounter = 0;
            int colcounter = 0;

            // Обход всех элементов
            foreach (UIElement child in this.Children)
            {
                // Получаем желаемый размер элемента
                child.Measure(childConstraint);

                // Обновляем максимальное значение стркои
                this.rows[rowcounter] = child.DesiredSize.Height < this.rows[rowcounter] ? this.rows[rowcounter] : child.DesiredSize.Height;
                
                // Обновляем максимальное значение столбца
                this.columns[colcounter] = child.DesiredSize.Width < this.columns[colcounter] ? this.columns[colcounter] : child.DesiredSize.Width;
                
                // Добавляем 10 для задания отступа
                this.columns[colcounter] = this.columns[colcounter] + 10;

                colcounter++;
                if (colcounter == realColumns)
                {
                    rowcounter++;
                    colcounter = 0;
                }
            }
            
            // Получаем совокупную высоты всех строки и ширину всех столбцов
            double panelHeight = this.rows.Sum();
            double panelWidth = this.columns.Sum();
            
            // На основании полученных значений устанавливаем размер панели
            return new Size(panelHeight, panelWidth);
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeSize">
        /// The arrange size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double cellWidth;
            double cellHeight;

            // Счетчики
            int rowcounter = 0;
            int colcounter = 0;

            // Текущие позиции
            double currentX = 0;
            double currentY = 0;

            // Обход всех элементов панели
            foreach (UIElement child in this.Children)
            {
                cellHeight = this.rows[rowcounter];
                cellWidth = this.columns[colcounter];
                
                // Определяем пространство для каждого дочернего элемента
                child.Arrange(new Rect(currentX, currentY, cellWidth, cellHeight));

                colcounter++;
                currentX += cellWidth;
                if (colcounter == realColumns)
                {
                    rowcounter++;
                    colcounter = 0;
                    currentY += cellHeight;
                    currentX = 0;
                }
            }
            
            // Возвращаем размер панели                       
            return arrangeSize;
        }

        /// <summary>
        /// TODO The calculate columns.
        /// </summary>
        private void CalculateColumns()
        {
            // Подсчет элементов
            double elementCount = this.Children.Count;
            
            // Если панель пуста, выходим из метода
            if (elementCount == 0) return;

            realRows = RowsNumber;
            realColumns = ColumnsNumber;

            // Если свойства rows и columns установлены, используем их
            if (this.realRows != 0 && this.realColumns != 0) return;

            // Если ни одно из свойств не установлено, вычисляем кол-во столбцов
            if (this.realColumns == 0 && realRows == 0)
                realColumns = (int)Math.Ceiling(Math.Sqrt(elementCount));

            // Если установлено только свойство rows, вычисляем свойство columns
            if (realColumns == 0)
                realColumns = (int)Math.Ceiling(elementCount / realRows);

            // Если установлено только свойство columns, вычисляем свойство rows
            if (realRows == 0)
                realRows = (int)Math.Ceiling(elementCount / realColumns);

            // Массив для значений высоты строк
            this.rows = new double[realRows];

            // Массив для значений ширины столбцов
            this.columns = new double[realColumns];
        }
    }
}
